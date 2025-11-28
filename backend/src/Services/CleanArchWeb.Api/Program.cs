using CleanArchWeb.Application;
using CleanArchWeb.Infrastructure;
using CleanArchWeb.Application.Weather;
using CleanArchWeb.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Expose OpenAPI JSON at /openapi/v1.json
    app.MapOpenApi();

    // Enable Swagger JSON and UI at /swagger and /swagger/index.html
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchWeb API v1");
        options.RoutePrefix = "swagger"; // UI at /swagger
    });
}

app.UseHttpsRedirection();

app.UseCors();

// Authorization middleware is not required for public sample endpoints

// Register API endpoints via modules for maintainability
app.MapWeatherForecastEndpoints();
app.MapChatEndpoints();

app.Run();

// Expose Program class for integration testing via WebApplicationFactory
public partial class Program { }
