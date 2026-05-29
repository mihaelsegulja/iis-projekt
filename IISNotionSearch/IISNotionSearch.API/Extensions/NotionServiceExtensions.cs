using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Services;
using IISNotionSearch.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.API.Extensions;

public static class NotionServiceExtensions
{
    public static IServiceCollection AddNotionServiceSwitch(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotionService>(provider =>
        {
            var appConfig = provider.GetRequiredService<IOptions<AppConfig>>().Value;
            if (appConfig.DataSource == DataSourceType.Local)
            {
                return provider.GetRequiredService<LocalNotionService>();
            }

            return provider.GetRequiredService<ExternalNotionService>();
        });

        return services;
    }
}
