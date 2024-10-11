using JwtImplementation.BLL.Model;

namespace JwtImplementation.BLL.Service
{
    public interface IAuthService
    {
        Task<string>  GenerateToken(User user);
        string RefreshToken();
    }
}
