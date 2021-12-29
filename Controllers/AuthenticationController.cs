using Electronic_Organizer_API.Dto;
using Electronic_Organizer_API.Models;
using Electronic_Organizer_API.Security;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Electronic_Organizer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ElectronicOrganizerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User with this E-mail already exists!" });
            var endUser = new EndUser()
            {
                Email = model.Email,
                Avatar = model.Avatar,
            };
            _context.EndUsers.Add(endUser);
            await _context.SaveChangesAsync();
            user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
            var passwordHashing = new PasswordHashing();
            var salt = passwordHashing.CreateSalt();
            var hashed = passwordHashing.GenerateHash(model.Password, salt);
            _context.EndUserSecurities.Add(new EndUserSecurity
            {
                Salt = salt,
                HashedPassword = hashed,
                EndUserId = user.Id
            });
            var result = await _context.SaveChangesAsync();
            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            return Ok(new ResponseDto { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            Console.WriteLine("Login");
            var user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                var passwordHashing = new PasswordHashing();
                var userSecurity = await _context.EndUserSecurities.FirstOrDefaultAsync(us => us.EndUserId == user.Id);
                if (userSecurity.Salt != null || userSecurity.HashedPassword != null)
                {
                    if (passwordHashing.IsPassowrdValid(model.Password, userSecurity.Salt, userSecurity.HashedPassword))
                    {
                        var token = JwtManager.GenerateJwtToken(user, _configuration);
                        return Ok(new ResponseDto { Status = "Success", Message = "Successfully logged in!", Token = token });
                    }
                }
            }
            return Unauthorized(new ResponseDto { Status = "Error", Message = "Wrong E-mail or password." });
        }

        [HttpPost]
        [Route("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto model)
        {
            Console.WriteLine("External-Login");
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Google:ClientId"] }
            };
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(model.TokenId, settings);
            var endUser = new EndUser();

            if (validPayload != null)
            {
                var user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == validPayload.Email);
                if (user != null)
                {
                    endUser = new EndUser()
                    {
                        Email = user.Email,
                        Avatar = user.Avatar,
                    };
                }
                else
                {
                    endUser = new EndUser()
                    {
                        Email = validPayload.Email,
                        Avatar = validPayload.Picture,
                    };
                    _context.EndUsers.Add(endUser);
                    await _context.SaveChangesAsync();
                    user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == validPayload.Email);
                    _context.EndUserSecurities.Add(new EndUserSecurity
                    {
                        Salt = null,
                        HashedPassword = null,
                        EndUserId = user.Id
                    });
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User creation failed!" });

                }
                var token = JwtManager.GenerateJwtToken(endUser, _configuration);
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully logged in!", Token = token });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "Invalid Token" });
        }
    }
}
