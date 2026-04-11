using IISNotionSearch.Application.Common.Interfaces.Security;
using IISNotionSearch.Application.Interfaces.Common;
using IISNotionSearch.Infrastructure.Security.Helpers;
using IISNotionSearch.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IISNotionSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordHelper, PasswordHelper>();
        services.AddScoped<ITokenHelper, TokenHelper>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}

