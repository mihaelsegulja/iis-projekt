using IISNotionSearch.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace IISNotionSearch.API.Abstractions.Attributes;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Roles[] explicitRoles)
    {
        if (explicitRoles.Length > 0)
        {
            Roles = string.Join(",", explicitRoles.Select(r => r.ToString()));
        }
    }
    
    private Roles _minRole;

    public Roles MinRole
    {
        get => _minRole;
        set
        {
            _minRole = value;

            var hierarchicalRoles = Enum.GetValues<Roles>()
                .Where(r => (int)r >= (int)value)
                .Select(r => r.ToString());

            if (string.IsNullOrEmpty(Roles))
            {
                Roles = string.Join(",", hierarchicalRoles);
            }
            else
            {
                var explicitList = Roles.Split(',');
                Roles = string.Join(",", explicitList.Concat(hierarchicalRoles).Distinct());
            }
        }
    }
}