using JwtImplementation.BLL.Model;

namespace JwtImplementation.BLL.Service
{
    public interface IUser
    {
        Task<User> RegisterUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<string> Login(string email, string password);
    }
}
