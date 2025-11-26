using CleanArchWeb.Application.Weather;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(IWeatherForecastService weatherService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var forecasts = weatherService.GetForecasts();
        return Ok(forecasts);
    }
}
