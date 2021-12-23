using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Electronic_Organizer_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Electronic_Organizer_API.Dto;
using Microsoft.Data.SqlClient;
using Dapper;
using Electronic_Organizer_API.Security;

namespace Electronic_Organizer_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EndUsersController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;

        public EndUsersController(ElectronicOrganizerContext context)
        {
            _context = context;
        }

        // Post: api/EndUsers/
        [HttpPost]
        public async Task<IActionResult> GetPasswordExistance([FromBody] UserDto model)
        {
            string sqlcmd = "EXEC eo_get_user_password_existence @email=@Mail";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail });
            var fullResult = string.Concat(result);
            if (int.Parse(result.First()) == 1)
                return Ok(new ResponseDto { Status = "Success", Message = "Password exists" });
            else
                return NoContent();
        }
        // Post: api/EndUsers/set-password
        [HttpPost]
        [Route("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] UserDto model)
        {
            if (model.Password == model.OldPassword)
                return BadRequest(new ResponseDto { Status = "Error", Message = "The changed password must be different." });
            string sqlcmd = $"EXEC eo_get_user_password_existence @email=@Mail";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail });
            sqlcmd = $"EXEC eo_set_password @email=@Mail, @salt=@Salt, @hashed_password=@HashedPassword";
            var passwordHashing = new PasswordHashing();
            var salt = passwordHashing.CreateSalt();
            var hashed = passwordHashing.GenerateHash(model.Password, salt);
            if (int.Parse(result.First()) == 0)
            {
                result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail, Salt = salt, HashedPassword = hashed });
                return Ok(new ResponseDto { Status = "Success", Message = "Password set successfully!" });
            }
            else
            {
                var user = await _context.EndUsers.FirstOrDefaultAsync(u => u.Email == model.UserMail);
                if (user != null)
                {
                    var userSecurity = await _context.EndUserSecurities.FirstOrDefaultAsync(us => us.EndUserId == user.Id);
                    if (passwordHashing.IsPassowrdValid(model.OldPassword, userSecurity.Salt, userSecurity.HashedPassword))
                    {
                        result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail, Salt = salt, HashedPassword = hashed });
                        return Ok(new ResponseDto { Status = "Success", Message = "Password changed successfully!" });
                    }
                    return BadRequest(new ResponseDto { Status = "Error", Message = "Old password is incorrect." });
                }

            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User not found" });
        }

        // Post: api/EndUsers/set-avatar
        [HttpPost]
        [Route("set-avatar")]
        public async Task<IActionResult> SetAvatar([FromBody] UserDto model)
        {
            string sqlcmd = "EXEC eo_set_avatar @user_mail=@Mail, @avatar=@Avatar";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail, Avatar = model.Avatar });
            if (!result.Any())
                return Ok(new ResponseDto { Status = "Success", Message = "Avatar changed successfully!" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "Error when changing avatar." });
        }
    }
}
