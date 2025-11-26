using CleanArchWeb.Domain.Weather;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchWeb.Application.Weather;

public interface IWeatherForecastService
{
    Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default);
}
