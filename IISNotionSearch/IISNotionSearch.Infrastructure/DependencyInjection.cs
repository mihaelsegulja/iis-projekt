using System.Net;
using System.Net.Sockets;
using IISNotionSearch.Application.Common.Interfaces.Security;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.Interfaces.Common;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Infrastructure.Security.Helpers;
using IISNotionSearch.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, DhmzConfig dhmzConfig)
    {
        var dhmzBaseUrl = dhmzConfig.BaseUrl;

        services.AddHttpClient<DhmzGrpcService>(client =>
        {
            client.BaseAddress = new Uri(dhmzBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) IISNotionSearch/1.0");
            client.DefaultRequestHeaders.Add("Accept", "text/xml");
        }).ConfigurePrimaryHttpMessageHandler(CreateDhmzHttpHandler);

        services.AddHttpClient<NotionHttpClient>((provider, client) =>
        {
            var config = provider.GetRequiredService<IOptions<NotionConfig>>().Value;
            client.BaseAddress = new Uri(config.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.InternalIntegrationSecret}");
            client.DefaultRequestHeaders.Add("Notion-Version", config.Version);
        });

        services.AddScoped<ExternalNotionService>();
        services.AddScoped<IImportService, ImportService>();

        services.AddScoped<IPasswordHelper, PasswordHelper>();
        services.AddScoped<ITokenHelper, TokenHelper>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    private static HttpMessageHandler CreateDhmzHttpHandler()
    {
        return new SocketsHttpHandler
        {
            ConnectCallback = async (context, ct) =>
            {
                var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, ct);
                var ipv4 = addresses.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

                if (ipv4.Length == 0)
                    ipv4 = addresses;

                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(ipv4, context.DnsEndPoint.Port, ct);
                return new NetworkStream(socket, ownsSocket: true);
            }
        };
    }
}
