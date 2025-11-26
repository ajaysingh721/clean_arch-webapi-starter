using CleanArchWeb.Infrastructure.Weather;
using FluentAssertions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchWeb.Api.Tests;

public class RandomWeatherForecastServiceTests
{
    [Fact]
    public async Task GetForecastsAsync_Default_ReturnsFiveItems()
    {
        var service = new RandomWeatherForecastService();
        var result = await service.GetForecastsAsync(CancellationToken.None);

        result.Should().HaveCount(5);
        result.All(r => r.TemperatureF == (int)System.Math.Round(32 + (r.TemperatureC / 0.5556))).Should().BeTrue();
    }
}