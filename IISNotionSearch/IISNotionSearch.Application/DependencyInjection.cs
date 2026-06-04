using Microsoft.Extensions.DependencyInjection;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Services;

namespace IISNotionSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<LocalNotionService>();

        return services;
    }
}
