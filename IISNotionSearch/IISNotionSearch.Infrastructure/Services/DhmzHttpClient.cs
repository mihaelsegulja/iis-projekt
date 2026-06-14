using System.Xml.Serialization;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Infrastructure.Models.Dhmz;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure.Services;

public class DhmzHttpClient
{
    private static readonly XmlSerializer Serializer = new(typeof(DhmzWeatherXml));

    private readonly HttpClient _httpClient;
    private readonly DhmzConfig _dhmzConfig;

    public DhmzHttpClient(HttpClient httpClient, IOptions<DhmzConfig> dhmzConfig)
    {
        _httpClient = httpClient;
        _dhmzConfig = dhmzConfig.Value;
    }

    public async Task<DhmzWeatherXml> FetchWeatherDataAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            _dhmzConfig.XmlPath,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var xmlStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return (DhmzWeatherXml)Serializer.Deserialize(xmlStream)!;
    }
}
