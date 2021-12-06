using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Electronic_Organizer_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Electronic_Organizer_API.Dto;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;
using Electronic_Organizer_API.Security;
using System.Data;

namespace Electronic_Organizer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndUsersController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;
        private readonly IConfiguration _configuration;

        public EndUsersController(ElectronicOrganizerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/EndUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndUser>>> GetEndUsers()
        {
            return await _context.EndUsers.ToListAsync();
        }

        // GET: api/EndUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EndUser>> GetEndUser(int id)
        {
            var endUser = await _context.EndUsers.FindAsync(id);

            if (endUser == null)
            {
                return NotFound();
            }

            return endUser;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] AuthDto model)
        {
            using SqlConnection connection = new(_context.Database.GetConnectionString());

            string sql = $"EXEC eo_get_user_by_email @email= '{model.Email}'";
            var res = connection.Query<string>(sql);
            if (res.Any())
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User already exists!" });
            var user = new EndUser()
            {
                Email = model.Email,
                Avatar = model.Avatar,
            };
            _context.EndUsers.Add(user);
            await _context.SaveChangesAsync();

            var passwordHashing = new PasswordHashing();
            var salt = passwordHashing.CreateSalt();
            var hashed = passwordHashing.GenerateHash(model.Password, salt);
            res = connection.Query<string>(sql);
            var fullResult = string.Concat(res);
            var userData = JsonConvert.DeserializeObject<List<EndUser>>(fullResult);
            _context.EndUserSecurities.Add(new EndUserSecurity
            {
                Salt = salt,
                HashedPassword = hashed,
                EndUserId = userData.First().Id
            });
            _context.Timetables.Add(new Timetable { EndUserId = userData.First().Id });
            var result = await _context.SaveChangesAsync();
            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            return Ok(new ResponseDto { Status = "Success", Message = "User created successfully!" });
        }
    }
}
