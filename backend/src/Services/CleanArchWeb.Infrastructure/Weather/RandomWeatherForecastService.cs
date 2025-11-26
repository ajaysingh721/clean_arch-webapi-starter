using CleanArchWeb.Application.Weather;
using CleanArchWeb.Domain.Weather;

namespace CleanArchWeb.Infrastructure.Weather;

public class RandomWeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public IReadOnlyList<WeatherForecast> GetForecasts()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var random = new Random();

        var items = Enumerable.Range(1, 5)
            .Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = random.Next(-20, 55),
                Summary = Summaries[random.Next(Summaries.Length)]
            })
            .ToList();

        return items;
    }
}
