using System.Text.Json.Serialization;

namespace IISNotionSearch.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;

    [JsonIgnore]
    public string RefreshToken { get; set; } = null!;
}
