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

    public Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        // Simulate fast in-memory generation; cancellation token considered for future extensibility.
        cancellationToken.ThrowIfCancellationRequested();

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var random = new Random();

        var items = Enumerable.Range(1, 5)
            .Select(index => WeatherForecast.Create(
                startDate.AddDays(index),
                random.Next(-20, 55),
                Summaries[random.Next(Summaries.Length)]))
            .ToList();

        return Task.FromResult<IReadOnlyList<WeatherForecast>>(items);
    }
}
