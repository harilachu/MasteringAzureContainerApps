namespace YarpGatewayService.AuthHandlers
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(string secret, string issuer, string audience);
    }
}
