using System.Xml.Serialization;
using Grpc.Core;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Infrastructure.Grpc;
using IISNotionSearch.Infrastructure.Models.Dhmz;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure.Services;

public class DhmzGrpcService : WeatherService.WeatherServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly DhmzConfig _dhmzConfig;

    public DhmzGrpcService(HttpClient httpClient, IOptions<DhmzConfig> dhmzConfig)
    {
        _httpClient = httpClient;
        _dhmzConfig = dhmzConfig.Value;
    }

    public override async Task<WeatherResponse> GetWeatherByCity(WeatherRequest request, ServerCallContext context)
    {
        var response = await GetWeatherByCityStandardAsync(request, context.CancellationToken);

        return response.Status switch
        {
            ResultStatus.Ok => response.Data!,
            ResultStatus.NotFound => throw new RpcException(new Status(StatusCode.NotFound, response.Message ?? "Not found.")),
            _ => throw new RpcException(new Status(StatusCode.Internal, response.Message ?? "Internal server error."))
        };
    }

    public async Task<StandardResponse<WeatherResponse>> GetWeatherByCityStandardAsync(WeatherRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(_dhmzConfig.TimeoutSeconds);
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var weatherData = await FetchWeatherDataAsync(linkedCts.Token);

            var filteredGradovi = weatherData.Gradovi
                .Where(g => g.GradIme.Contains(request.CityName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredGradovi.Count == 0)
            {
                return StandardResponse<WeatherResponse>.Create(
                    ResultStatus.NotFound,
                    message: $"No weather data found for city '{request.CityName}'.");
            }

            var resultResponse = new WeatherResponse
            {
                LastUpdated = $"{weatherData.DatumTermin.Datum} {weatherData.DatumTermin.Termin}:00"
            };

            foreach (var grad in filteredGradovi)
            {
                resultResponse.Results.Add(new CityWeather
                {
                    CityName = grad.GradIme,
                    Temperature = grad.Podatci.Temp.Trim(),
                    Humidity = grad.Podatci.Vlaga.Trim(),
                    Condition = grad.Podatci.Vrijeme.Trim(),
                    WindSpeed = grad.Podatci.VjetarBrzina.Trim(),
                    WindDirection = grad.Podatci.VjetarSmjer.Trim()
                });
            }

            return StandardResponse<WeatherResponse>.Create(ResultStatus.Ok, resultResponse);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return StandardResponse<WeatherResponse>.Create(
                ResultStatus.InternalError,
                message: "Weather request was canceled by the client.");
        }
        catch (OperationCanceledException)
        {
            return StandardResponse<WeatherResponse>.Create(
                ResultStatus.InternalError,
                message: $"DHMZ request timed out after {_dhmzConfig.TimeoutSeconds:0} seconds.");
        }
        catch (HttpRequestException ex)
        {
            return StandardResponse<WeatherResponse>.Create(
                ResultStatus.InternalError,
                message: $"Error calling DHMZ endpoint: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StandardResponse<WeatherResponse>.Create(
                ResultStatus.InternalError,
                message: $"Error fetching DHMZ data: {ex.Message}");
        }
    }

    private async Task<DhmzWeatherXml> FetchWeatherDataAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            _dhmzConfig.XmlPath,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var xmlStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var serializer = new XmlSerializer(typeof(DhmzWeatherXml));
        return (DhmzWeatherXml)serializer.Deserialize(xmlStream)!;
    }
}
