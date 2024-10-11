using JwtImplementation.BLL.Model;
using JwtImplementation.BLL.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq;
using System.Security;
using System.Security.Claims;

public class PermissionService : IPermissionService
{
    public string HasPermission(ClaimsPrincipal user, string permission)
    {
        var permissionsClaim = user.Claims.FirstOrDefault(c => c.Type == "Permissions");
        if (permissionsClaim != null)
        {
            var permissions = permissionsClaim.Value.Contains("Read");
            return permissions.ToString();
        }
        return "Unauthorized";
    }
    public bool PermisssionAccess()
    {
        return true;
    }
}
