﻿using System;
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


        //PUT: api/Services/
        [HttpPut]
        public async Task<IActionResult> PutService([FromBody] ServiceDto model)
        {
            string sqlcmd = $"EXEC eo_services_update @title=@Title, @estimated_time= {model.EstimatedTime}, @service_code='{model.ServiceCode}', @user_mail='{model.UserMail}', @service_id={model.ServiceId}";

            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { model.Title } );
            var fullResult = string.Concat(result);
            if (fullResult == "Success")
                return Ok(new ResponseDto { Status = "Success", Message = fullResult });
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = fullResult });
        }

        // DELETE: api/Services/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new ResponseDto { Status = "Error", Message = "Service not found." });
            }
            string sqlcmd = $"EXEC eo_services_update_after_delete @id=@Id";
            var result = await connection.QueryAsync<string>(sqlcmd, new { Id=id });
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
