using JwtImplementation.BLL.Model;
using JwtImplementation.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace JwtImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IActionResult> GetUserall()
        {
            var allusers = await _userRepository.GetAllUser();
            if (allusers != null)

                return Ok(allusers);
            else
                return Ok("User not found");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser(string email)
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type == "Permissions");
            var userbyemail = await _userRepository.GetUserByEmail(email);
            if (userbyemail != null)
            {
                return Ok(userbyemail);
            }
            else
                return Ok("User not found");
        }
        [Authorize]
        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUser()
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type=="Permissions");
            if (permission != null && permission.Value.Contains("Read")) 
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var userbyemail = await _userRepository.GetUserByEmail(email);
                if (userbyemail != null)
                    return Ok(userbyemail);
                else
                    return Ok("User not found");
            }
            if(permission != null && permission.Value.Contains("Read,Write"))
            {
                var allusers = await _userRepository.GetAllUser();
                if (allusers != null)

                    return Ok(allusers);
                else
                    return Ok("User not found");
            }
            return Unauthorized();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Invalid user data.");
            }

            var newUser = new User
            {
                Email = user.Email,
                UserName = user.UserName,
                Password = user.Password,  
                RoleId = user.RoleId
            };

            var registeredUser = await _userRepository.RegisterUser(newUser);

            if (registeredUser != null)
            {
                return Ok(new { Message = "User registered successfully.", User = registeredUser });
            }

            return BadRequest("User registration failed. The user may already exist.");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email,string password)
        {
            var jwttoken = await _userRepository.Login(email, password);
            if (jwttoken == null) 
                Unauthorized();
             return Ok(new {token=jwttoken});
        }
        private IActionResult GetClaims()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var claims = jwtToken.Claims.Select(c => new { c.Type, c.Value });

                return Ok(claims);
            }

            return BadRequest("Invalid token.");
        }
    }
}
