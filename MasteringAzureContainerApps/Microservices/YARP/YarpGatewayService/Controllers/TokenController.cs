using ERP.Common.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using YarpGatewayService.AuthHandlers;

namespace YarpGatewayService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<TokenController> _logger;
        private readonly YarpConfig _yarpConfig;

        public TokenController(ITokenGenerator tokenGenerator, ILogger<TokenController> logger, IOptions<YarpConfig> yarpConfig)
        {
            _tokenGenerator = tokenGenerator;
            _logger = logger;
            _yarpConfig = yarpConfig.Value;
        }

        [HttpPost("generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateToken(string apiKey)
        {
            try
            {
                if (apiKey != _yarpConfig.YarpApiKey)
                {
                    return Unauthorized("Invalid API key");
                }

                var token = await _tokenGenerator.GenerateTokenAsync(_yarpConfig.Token.AppSecret, _yarpConfig.Token.Issuer, _yarpConfig.Token.Audience);
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Failed to generate token");
                }
                return Ok(new { AccessToken = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token");
                return BadRequest("Failed to generate token");
            }
        }
    }
}
