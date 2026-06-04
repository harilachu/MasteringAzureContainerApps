using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace YarpGatewayService.AuthHandlers
{
    public class JWTTokenGenerator : ITokenGenerator
    {
        private readonly ILogger<JWTTokenGenerator> _logger;

        public JWTTokenGenerator(ILogger<JWTTokenGenerator> logger)
        {
            _logger = logger;
        }
        public async Task<string> GenerateTokenAsync(string secret, string issuer, string audience)
        {
            var token = await Task.Run(() => CreateTokenAsync(secret, issuer, audience));
            return token;
        }

        private async Task<string> CreateTokenAsync(string secret, string issuer, string audience)
        {
            try
            {
                // Set token expiry to 1 hour from now
                DateTimeOffset expiryUnixOffset = DateTimeOffset.UtcNow.AddHours(1);
                long expiryUnixTimeSeconds = expiryUnixOffset.ToUnixTimeSeconds();

                // Hash the secret using SHA256 to create a fixed-length key
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(secret);
                var key = sha.ComputeHash(bytes);
                var secretHash = Convert.ToHexString(key).ToLowerInvariant();

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretHash)); //Use the secret hash as the signing key
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim("scp", "employee-api-access"),
                        new System.Security.Claims.Claim("scp", "review-api-access"),
                        // Add other claims as needed
                    }),
                    Expires = DateTimeOffset.FromUnixTimeSeconds(expiryUnixTimeSeconds).UtcDateTime,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token");
                return string.Empty;
            }
        }
    }
}
