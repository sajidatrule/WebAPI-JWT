using JwtImplementation.BLL.Service;
using JwtImplementation.DAL.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace JwtImplementation.Configuration
{
    public class ServiceConfigurations
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<UserRepository>();
        }
    }
}
