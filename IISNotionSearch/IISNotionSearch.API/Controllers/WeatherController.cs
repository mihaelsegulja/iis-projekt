using IISNotionSearch.API.Abstractions.Controllers;
using IISNotionSearch.Application.Models;
using Grpc.Core;
using IISNotionSearch.Infrastructure.Grpc;
using Microsoft.AspNetCore.Mvc;

namespace IISNotionSearch.API.Controllers;

public class WeatherController : BaseController
{
    private readonly WeatherService.WeatherServiceClient _grpcClient;

    public WeatherController(WeatherService.WeatherServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] string? cityName)
    {
        var request = new WeatherRequest { CityName = cityName ?? string.Empty };

        try
        {
            var grpcResponse = await _grpcClient.GetWeatherByCityAsync(request, cancellationToken: HttpContext.RequestAborted);
            var response = StandardResponse<WeatherResponse>.Create(ResultStatus.Ok, grpcResponse);
            return HandleResponse(response);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            var response = StandardResponse<WeatherResponse>.Create(ResultStatus.NotFound, message: ex.Status.Detail);
            return HandleResponse(response);
        }
        catch (RpcException ex)
        {
            var response = StandardResponse<WeatherResponse>.Create(ResultStatus.InternalError, message: ex.Status.Detail);
            return HandleResponse(response);
        }
    }
}
