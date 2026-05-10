using IISNotionSearch.Domain.Entities;
using IISNotionSearch.Domain.Interfaces;
using IISNotionSearch.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IISNotionSearch.Repository.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return DbSet.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
    }
}
