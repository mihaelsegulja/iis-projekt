using IISNotionSearch.Domain.Abstractions;
using IISNotionSearch.Domain.Entities;

namespace IISNotionSearch.Domain.Interfaces;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}

