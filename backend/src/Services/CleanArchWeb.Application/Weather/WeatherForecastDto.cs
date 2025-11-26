using System;

namespace CleanArchWeb.Application.Weather;

public sealed record WeatherForecastDto(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);