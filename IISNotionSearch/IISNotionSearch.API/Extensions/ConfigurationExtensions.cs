using IISNotionSearch.Application.Configurations;

namespace IISNotionSearch.API.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
        services.Configure<CorsConfig>(configuration.GetSection("CorsConfig"));
        services.Configure<NotionConfig>(configuration.GetSection("NotionConfig"));
        services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
        services.Configure<DhmzConfig>(configuration.GetSection("DhmzConfig"));
        services.Configure<GrpcConfig>(configuration.GetSection("GrpcConfig"));

        return services;
    }

    public static string GetConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("Db")!;
    }
}
