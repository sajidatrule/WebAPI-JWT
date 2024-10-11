using Azure.Core;
using JwtImplementation.BLL.Model;
using JwtImplementation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtImplementation.BLL.Service
{
    public class AuthService:IAuthService
    {
        private readonly IConfiguration config;
        private readonly ApplicationDbContext _context;
        public AuthService(IConfiguration config,ApplicationDbContext dbContext)
        {
            this.config = config;
            _context = dbContext;
        }

        public async Task<string> GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var roleid = user.RoleId;
            var roleName = _context.Roles
                .Where(r => r.Id == roleid)
                .Select(r => r.RoleName)
                .FirstOrDefault();

            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,roleName),
                new Claim("Permissions", user.Permissions)
            };
            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                claims:   claim,
                expires : DateTime.Now.AddDays(1),
                signingCredentials:credentials
            );
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenstring = tokenhandler.WriteToken(token);

            return await Task.FromResult(tokenstring);
        }

        public string RefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        //public async Task<string> CliamCheck()
        //{
        //    var permission = User.Claims.FirstOrDefault(c => c.Type == "Permissions");
        //    //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var handler = new JwtSecurityTokenHandler();

        //    if (handler.CanReadToken(token))
        //    {
        //        var jwtToken = handler.ReadJwtToken(token);
        //        var claims = jwtToken.Claims.Select(c => new { c.Type, c.Value });

        //        return Ok(claims);
        //    }

        //    return BadRequest("Invalid token.");
        //}
    }
}
