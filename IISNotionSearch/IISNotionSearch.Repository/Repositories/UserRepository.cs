using IISNotionSearch.Domain.Entities;
using IISNotionSearch.Domain.Interfaces;
using IISNotionSearch.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IISNotionSearch.Repository.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return DbSet.FirstOrDefaultAsync(u => u.Username == username);
    }
}