using CleanArchWeb.Domain.Weather;

using System.Collections.Generic;
namespace CleanArchWeb.Application.Weather;

public interface IWeatherForecastService
{
    IReadOnlyList<WeatherForecast> GetForecasts();
}
