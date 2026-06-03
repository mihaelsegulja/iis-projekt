using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using IISNotionSearch.Application.Common.Interfaces.Security;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.Interfaces.Common;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Infrastructure.Security.Helpers;
using IISNotionSearch.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<DhmzGrpcService>(client =>
        {
            client.BaseAddress = new Uri("https://vrijeme.hr/");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) IISNotionSearch/1.0");
            client.DefaultRequestHeaders.Add("Accept", "text/xml");
        }).ConfigurePrimaryHttpMessageHandler(CreateDhmzHttpHandler);

        services.Configure<NotionConfig>(configuration.GetSection("NotionConfig"));

        services.AddHttpClient<NotionHttpClient>((provider, client) =>
        {
            var config = provider.GetRequiredService<IOptions<NotionConfig>>().Value;
            var baseUrl = config.BaseUrl;
            if (!baseUrl.EndsWith('/'))
            {
                baseUrl += '/';
            }
            client.BaseAddress = new Uri(baseUrl);
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
            ConnectCallback = async (context, cancellationToken) =>
            {
                var host = context.DnsEndPoint.Host;
                var port = context.DnsEndPoint.Port;
                var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
                var orderedAddresses = addresses
                    .OrderByDescending(address => address.AddressFamily == AddressFamily.InterNetwork)
                    .ToList();

                Exception? lastException = null;

                foreach (var address in orderedAddresses)
                {
                    var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                    {
                        NoDelay = true
                    };

                    try
                    {
                        await socket.ConnectAsync(new IPEndPoint(address, port), cancellationToken);
                        Stream stream = new NetworkStream(socket, ownsSocket: true);

                        if (context.InitialRequestMessage?.RequestUri?.Scheme == Uri.UriSchemeHttps)
                        {
                            var sslStream = new SslStream(stream, leaveInnerStreamOpen: false);
                            await sslStream.AuthenticateAsClientAsync(
                                new SslClientAuthenticationOptions
                                {
                                    TargetHost = host,
                                    EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                                },
                                cancellationToken);

                            return sslStream;
                        }

                        return stream;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        socket.Dispose();
                    }
                }

                throw new HttpRequestException($"Unable to connect to {host}:{port}", lastException);
            }
        };
    }
}
