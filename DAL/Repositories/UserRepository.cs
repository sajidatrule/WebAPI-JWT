using JwtImplementation.BLL.Model;
using JwtImplementation.BLL.Service;
using JwtImplementation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System.Runtime.CompilerServices;

namespace JwtImplementation.DAL.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        public UserRepository(ApplicationDbContext context,IAuthService authService) 
        {
            _context = context;
            _authService = authService;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e=>e.Email==email);
            if (user != null)
            {
                return user;
            }
            else 
            {
                return null;
            }
        }
       public async Task<List<User>> GetAllUser()
       {
            var getusers = await _context.Users.ToListAsync();
            var userlist = new List<User>();
            foreach (var user in getusers)
                if(user != null) 
                    userlist.Add(user);
            return userlist;
       }

        public async Task<string> Login(string email, string password)
        {
            var loginuser = _context.Users.SingleOrDefault(e=>e.Email==email);
            if (loginuser != null && loginuser.Password == password)
            {
                var token= await _authService.GenerateToken(loginuser);
                return token.ToString();
            }
            else
                return null;
        }

        public async Task<User> RegisterUser(User user)
        {
            var emailexists = await _context.Users.AnyAsync(e=>e.Email==user.Email);
            var userexists = await _context.Users.AnyAsync(u=>u.Id==user.Id);
            if(!emailexists && !userexists)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("User already exists");
                return null;
            }
            return user;
        }

    }
}
