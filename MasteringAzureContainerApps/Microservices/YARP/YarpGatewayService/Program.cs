using ERP.Common.Core;
using YarpGatewayService;
using YarpGatewayService.AuthHandlers;
using YarpGatewayService.AuthRegistry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<YarpConfig>(builder.Configuration.GetSection("YarpConfig"));
var yarpConfig = builder.Configuration.GetSection("YarpConfig").Get<YarpConfig>();
//Add Multiple authentication schemes - JWT | API Key | Entra ID (Azure AD B2C)
builder.Services.AddJWTAuthentication();

builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Ok"));
builder.Services.AddReverseProxyForYarp();
//Generate Custom JWT Tokens
builder.Services.AddScoped<ITokenGenerator, JWTTokenGenerator>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();
app.Run();
