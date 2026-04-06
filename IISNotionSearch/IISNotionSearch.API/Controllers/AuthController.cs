using IISNotionSearch.API.Abstractions;
using IISNotionSearch.Application.DTOs.Auth;
using IISNotionSearch.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace IISNotionSearch.API.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return HandleResponse(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var response = await _authService.RegisterAsync(request);
        return HandleResponse(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        return HandleResponse(response);
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut([FromBody] RefreshTokenRequestDto request)
    {
        var response = await _authService.SignOutAsync(request);
        return HandleResponse(response);
    }
}