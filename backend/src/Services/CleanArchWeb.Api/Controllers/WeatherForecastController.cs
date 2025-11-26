using CleanArchWeb.Application.Weather;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(IWeatherForecastService weatherService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var forecasts = await weatherService.GetForecastsAsync(cancellationToken);
        var dtos = forecasts.Select(f => f.ToDto());
        return Ok(dtos);
    }
}
