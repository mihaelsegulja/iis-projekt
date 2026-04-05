using IISNotionSearch.Domain.Enums;

namespace IISNotionSearch.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public Roles Role { get; set; } = Roles.User;
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}