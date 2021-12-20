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

namespace Electronic_Organizer_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;

        public ServicesController(ElectronicOrganizerContext context)
        {
            _context = context;
        }

        // Post: api/Services/list-services
        [HttpPost]
        [Route("list-services")]
        public async Task<IActionResult> GetServices([FromBody] ServiceDto model)
        {
            string sqlcmd = $"EXEC eo_services_select @user_mail='{model.UserMail}'";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd);
            var fullResult = string.Concat(result);

            if (result.Any())
            {
                var services = fullResult;
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully list services!", Data = services });
            }
            return NoContent();
        }
        // Post: api/Services/
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] ServiceDto model)
        {
            string sqlcmd = $"EXEC eo_services_insert @title='{model.Title}', @estimated_time= {model.EstimatedTime}, @service_code='{model.ServiceCode}', @user_mail='{model.UserMail}'";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd);
            var fullResult = string.Concat(result);

            if (fullResult=="Success")
                return Ok(new ResponseDto { Status = "Success", Message = fullResult });
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = fullResult});
        }


        // PUT: api/Services/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut]
        //public async Task<IActionResult> PutService([FromBody] ServiceDto model)
        //{
        //    if (id != service.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(service).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ServiceExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Services
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Service>> PostService(Service service)
        //{
        //    _context.Services.Add(service);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetService", new { id = service.Id }, service);
        //}

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new ResponseDto { Status = "Error", Message = "Service not found." });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
