using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IISNotionSearch.Application.Interfaces.Helpers;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Services;
using IISNotionSearch.Application.Services.Helpers;

namespace IISNotionSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordHelper, PasswordHelper>();
        services.AddScoped<ITokenHelper, TokenHelper>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
