using CleanArchWeb.Api.Controllers;
using CleanArchWeb.Application.Weather;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CleanArchWeb.Domain.Weather;
using Xunit;

namespace CleanArchWeb.Api.Tests;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsOk_WithForecasts()
    {
        var service = new Mock<IWeatherForecastService>();
        service.Setup(s => s.GetForecasts()).Returns(Array.Empty<WeatherForecast>());

        var controller = new WeatherForecastController(service.Object);

        var result = controller.Get();

        result.Should().BeOfType<OkObjectResult>();
    }
}

