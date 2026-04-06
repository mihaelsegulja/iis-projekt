namespace IISNotionSearch.Application.Interfaces.Helpers;

public interface IPasswordHelper
{
    string HashPassword(string password, string salt);
    string GenerateSalt();
    bool VerifyPassword(string password, string hash);
}

