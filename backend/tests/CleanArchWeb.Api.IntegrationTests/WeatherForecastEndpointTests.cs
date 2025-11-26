using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CleanArchWeb.Api.IntegrationTests;

public class WeatherForecastEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => { });
    }

    [Fact]
    public async Task GetWeatherForecasts_WhenCalled_ReturnsFiveValidDtos()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/WeatherForecast");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<WeatherForecastResponse>>();
        items.Should().NotBeNull();
        items!.Count.Should().Be(5);
        items.All(i => i.TemperatureF == (int)Math.Round(32 + (i.TemperatureC / 0.5556))).Should().BeTrue();
    }

    private sealed record WeatherForecastResponse(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);
}
