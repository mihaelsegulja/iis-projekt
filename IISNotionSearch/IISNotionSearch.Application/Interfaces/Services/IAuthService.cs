using IISNotionSearch.Application.DTOs.Auth;
using IISNotionSearch.Application.Models;

namespace IISNotionSearch.Application.Interfaces.Services;

public interface IAuthService
{
    Task<StandardResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<StandardResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<StandardResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<StandardResponse<bool>> SignOutAsync(RefreshTokenRequestDto request);
}
