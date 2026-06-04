using ERP.Common.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace YarpGatewayService.AuthRegistry
{
    public static class JWTAuthentication
    {
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services)
        {
            var tmpProvider = services.BuildServiceProvider();
            var yarpConfig = tmpProvider.GetRequiredService<IOptions<YarpConfig>>().Value;

            // Prevent JWT handler from remapping claim types (common gotcha for "scp"/"scope")
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Generate a symmetric security key from the secret key in app settings
            using var sha = SHA256.Create();
            var secretKey = yarpConfig.Token.AppSecret;
            var bytes = Encoding.UTF8.GetBytes(secretKey);
            var key = sha.ComputeHash(bytes);
            var secretHash = Convert.ToHexString(key).ToLowerInvariant();

            services.AddAuthentication() //Do not use default auth schemes names. Use different names to support multiple authentication.
                .AddJwtBearer("JWT_TOKEN_AUTH_SCHEME");

            services.AddOptions<JwtBearerOptions>("JWT_TOKEN_AUTH_SCHEME")
                .Configure<IOptions<YarpConfig>>((options, yarpConfig) =>
                {
                    options.RequireHttpsMetadata = false; // Set to true in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretHash)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = yarpConfig.Value.Token.Issuer,
                        ValidAudience = yarpConfig.Value.Token.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JWTAuthentication");
                            logger.LogError(context.Exception, "Authentication failed. Path: {Path}, Scheme: {Scheme}", context.HttpContext.Request.Path, context.Scheme);

                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }
                            context.Response.Headers.Append("X-Auth-Failed", "JWT_TOKEN_AUTH_SCHEME");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JWTAuthentication");
                            var claims = string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}:{c.Value}"));
                            logger.LogInformation("Token validated. User authenticated: {Authenticated}. Claims: {Claims}", context.Principal.Identity?.IsAuthenticated ?? false, claims);
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JWTAuthentication");
                            logger.LogWarning("Forbidden response. Path: {Path}, User: {User}", context.HttpContext.Request.Path, context.HttpContext.User.Identity?.Name ?? "Anonymous");
                            context.Response.Headers.Append("X-Auth-Forbidden", "JWT_TOKEN_AUTH_SCHEME");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                // Use this policy in Yarp reverse proxy route configuration to protect the routes with JWT authentication
                // Refer ReverseProxy.cs for the usage of this policy in the route configuration.
                options.AddPolicy("JWTAuthPolicy", policy =>
                {
                    policy.AddAuthenticationSchemes("JWT_TOKEN_AUTH_SCHEME")
                          //.RequireAuthenticatedUser()
                          .RequireAssertion(context =>
                          {
                              // Ensure the principal is authenticated
                              if (!(context.User.Identity?.IsAuthenticated ?? false))
                                  return false;

                              // Collect scope values from multiple possible claim names and split space-delimited values
                              var scopeValues = context.User.FindAll("scp").Select(c => c.Value)
                                                           .Concat(context.User.FindAll("scope").Select(c => c.Value))
                                                           .Concat(context.User.FindAll(ClaimConstants.Scope).Select(c => c.Value))
                                                           .Concat(context.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value))
                                                           .SelectMany(v => v.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                                           .Distinct(StringComparer.OrdinalIgnoreCase);

                              return scopeValues.Contains("employee-api-access") || scopeValues.Contains("review-api-access");
                          });
                });
            });
            return services;
        }
    }
}
