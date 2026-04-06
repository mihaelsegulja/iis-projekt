using IISNotionSearch.Domain.Entities;

namespace IISNotionSearch.Application.Interfaces.Helpers;

public interface ITokenHelper
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

