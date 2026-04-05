using IISNotionSearch.Domain.Entities;
using IISNotionSearch.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IISNotionSearch.Repository.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }
}