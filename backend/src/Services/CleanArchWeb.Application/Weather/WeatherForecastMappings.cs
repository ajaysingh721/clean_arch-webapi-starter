using CleanArchWeb.Domain.Weather;

namespace CleanArchWeb.Application.Weather;

public static class WeatherForecastMappings
{
    public static WeatherForecastDto ToDto(this WeatherForecast model)
        => new(model.Date, model.TemperatureC, model.TemperatureF, model.Summary);
}