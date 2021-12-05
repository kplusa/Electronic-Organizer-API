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

namespace Electronic_Organizer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndUsersController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;
        private readonly UserManager<EndUser> _userManager;
        //private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public EndUsersController(ElectronicOrganizerContext context, UserManager<EndUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
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

        // PUT: api/EndUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndUser(int id, EndUser endUser)
        {
            if (id != endUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(endUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EndUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EndUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EndUser>> PostEndUser(EndUser endUser)
        {
            _context.EndUsers.Add(endUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndUser", new { id = endUser.Id }, endUser);
        }

        // DELETE: api/EndUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndUser(int id)
        {
            var endUser = await _context.EndUsers.FindAsync(id);
            if (endUser == null)
            {
                return NotFound();
            }

            _context.EndUsers.Remove(endUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EndUserExists(int id)
        {
            return _context.EndUsers.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] AuthDto model)
        {
            //string sql = $"EXEC eo_get_user_by_email @email= {model.Email}";
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User already exists!" });
            EndUser user = new()
            {
                Email = model.Email,
                Avatar = model.Avatar,
            };
            _context.EndUsers.Add(user);
            _ = await _context.SaveChangesAsync();
            _ = await _userManager.FindByNameAsync(model.Email);
            //using SqlConnection connection = new(_context.Database.GetConnectionString());
            //var res = connection.Query<string>(sql);
            //var userData = JsonConvert.DeserializeObject<List<EndUser>>(res.First());
            //_context.EndUserSecurities.Add(new EndUserSecurity
            //{
            //    Salt = salt,
            //    HashedPassword = hashed,
            //    EndUserId = userData.First().Id
            //}
            //           );
            await _context.SaveChangesAsync();
            var resultt = await _userManager.CreateAsync(user, model.Password);
            if (!resultt.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            return Ok(new ResponseDto { Status = "Success", Message = "User created successfully!" });
        }
    }
}
