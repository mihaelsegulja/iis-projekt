using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Services;

namespace IISNotionSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
