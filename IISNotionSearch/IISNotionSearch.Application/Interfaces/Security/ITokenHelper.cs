using IISNotionSearch.Domain.Entities;

namespace IISNotionSearch.Application.Common.Interfaces.Security;

public interface ITokenHelper
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

