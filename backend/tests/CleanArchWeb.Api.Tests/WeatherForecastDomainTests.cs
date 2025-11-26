using CleanArchWeb.Domain.Weather;
using FluentAssertions;
using System;
using Xunit;

namespace CleanArchWeb.Api.Tests;

public class WeatherForecastDomainTests
{
    [Fact]
    public void Create_ValidInputs_ReturnsInstanceWithComputedFahrenheit()
    {
        var forecast = WeatherForecast.Create(new DateOnly(2025, 1, 1), 0, "Freezing");
        forecast.TemperatureF.Should().Be(32); // 0C -> 32F
    }

    [Fact]
    public void Create_InvalidTemperature_ThrowsArgumentOutOfRange()
    {
        Action act = () => WeatherForecast.Create(DateOnly.FromDateTime(DateTime.UtcNow), 101, null);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}