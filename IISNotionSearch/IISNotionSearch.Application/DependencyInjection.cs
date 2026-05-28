using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Services;

namespace IISNotionSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppConfig>(options => configuration.GetSection("AppConfig").Bind(options));
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<LocalNotionService>();

        return services;
    }
}
