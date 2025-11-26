using CleanArchWeb.Application.Weather;
using CleanArchWeb.Infrastructure.Weather;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastService, RandomWeatherForecastService>();
        return services;
    }
}
