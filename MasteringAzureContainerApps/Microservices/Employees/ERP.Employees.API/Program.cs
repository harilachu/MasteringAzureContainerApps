using ERP.Employees.API.Generated;
using Synaptrix;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Single call registers all handlers + wires GeneratedMediator as IMediator/ISender.
builder.Services.AddSynaptrixHandlers();

// If you also need a notification publisher for the reflection-based Mediator (non-generated path):
builder.Services.AddSynaptrixCoreServices();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
