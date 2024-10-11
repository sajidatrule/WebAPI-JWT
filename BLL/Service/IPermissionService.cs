using System.Security.Claims;

namespace JwtImplementation.BLL.Service
{
    public interface IPermissionService
    {
       string HasPermission(ClaimsPrincipal user, string permission);
    }

}
