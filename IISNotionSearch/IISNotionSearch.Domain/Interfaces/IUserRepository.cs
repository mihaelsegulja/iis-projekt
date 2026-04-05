using IISNotionSearch.Domain.Entities;

namespace IISNotionSearch.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}