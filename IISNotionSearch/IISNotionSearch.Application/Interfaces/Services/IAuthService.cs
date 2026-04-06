using IISNotionSearch.Application.Abstractions;
using IISNotionSearch.Application.DTOs.Auth;

namespace IISNotionSearch.Application.Interfaces.Services;

public interface IAuthService
{
    Task<StandardResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<StandardResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<StandardResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<StandardResponse<bool>> SignOutAsync(RefreshTokenRequestDto request);
}
