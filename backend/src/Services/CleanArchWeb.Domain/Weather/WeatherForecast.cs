namespace CleanArchWeb.Domain.Weather;

// Value object representing a single forecast snapshot.
// Created via factory to enforce invariants.
public class WeatherForecast
{
    public DateOnly Date { get; }
    public int TemperatureC { get; }
    public string? Summary { get; }

    // Derived value; domain logic, not stored.
    public int TemperatureF => (int)Math.Round(32 + (TemperatureC / 0.5556));

    private WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    public static WeatherForecast Create(DateOnly date, int temperatureC, string? summary)
    {
        // Basic validation (extend as domain evolves)
        if (temperatureC < -100 || temperatureC > 100)
            throw new ArgumentOutOfRangeException(nameof(temperatureC), "TemperatureC must be between -100 and 100.");
        return new WeatherForecast(date, temperatureC, summary);
    }
}
