using CleanArchWeb.Application.Weather;

namespace CleanArchWeb.Api.Endpoints;

public static class WeatherForecastEndpoints
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/WeatherForecast");

        group.MapGet("/", async (IWeatherForecastService service, CancellationToken ct) =>
        {
            var forecasts = await service.GetForecastsAsync(ct);
            var dtos = forecasts.Select(f => f.ToDto());
            return Results.Ok(dtos);
        });

        return routes;
    }
}