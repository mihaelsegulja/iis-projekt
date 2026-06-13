using HotChocolate.Authorization;
using IISNotionSearch.Domain.Enums;

namespace IISNotionSearch.API.GraphQL;

public class AuthorizeMinRoleAttribute : AuthorizeAttribute
{
    public AuthorizeMinRoleAttribute(Roles minRole)
    {
        Roles = Enum.GetValues<Roles>()
            .Where(r => (int)r >= (int)minRole)
            .Select(r => r.ToString())
            .ToArray();
    }
}
