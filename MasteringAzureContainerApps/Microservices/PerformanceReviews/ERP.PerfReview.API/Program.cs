using ERP.Common.Core;
using ERP.PerfReview.API;
using ERP.PerfReview.API.Generated;
using ERP.PerfReview.Application;
using ERP.PerfReview.Infrastructure.ServiceDependencies;
using Synaptrix;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Single call registers all handlers + wires GeneratedMediator as IMediator/ISender.
builder.Services.AddSynaptrixHandlers()
    .AddSynaptrixCoreServices();

builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));
var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>();

builder.Services.AddApplicationServices()
    .AddInfrastructureServices(appConfig);

builder.Services.AddControllers();
builder.Services.AddHealthChecks()
.AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Ok"));

builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseExceptionHandler(options => { });
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
