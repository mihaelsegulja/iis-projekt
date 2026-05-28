using System.Xml.Serialization;
using Grpc.Core;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Infrastructure.Grpc;
using IISNotionSearch.Infrastructure.Models.Dhmz;

namespace IISNotionSearch.Infrastructure.Services;

public class DhmzGrpcService : WeatherService.WeatherServiceBase
{
    private static readonly TimeSpan DhmzRequestTimeout = TimeSpan.FromSeconds(15);
    private readonly HttpClient _httpClient;

    public DhmzGrpcService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
            using var timeoutCts = new CancellationTokenSource(DhmzRequestTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            var token = linkedCts.Token;

            var response = await _httpClient.GetAsync(
                "hrvatska_n.xml",
                HttpCompletionOption.ResponseHeadersRead,
                token);

            response.EnsureSuccessStatusCode();

            var xmlStream = await response.Content.ReadAsStreamAsync(token);
            var serializer = new XmlSerializer(typeof(DhmzWeatherXml));
            var weatherData = (DhmzWeatherXml)serializer.Deserialize(xmlStream)!;

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
                message: $"DHMZ request timed out after {DhmzRequestTimeout.TotalSeconds:0} seconds.");
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
}
