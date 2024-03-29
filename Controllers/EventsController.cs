﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Electronic_Organizer_API.Models;
using Electronic_Organizer_API.Dto;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Electronic_Organizer_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;

        public EventsController(ElectronicOrganizerContext context)
        {
            _context = context;
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("count-time")]
        public async Task<ActionResult<Event>> CountEndTime([FromBody] EventDto model)
        {
            string sqlcmd = "EXEC eo_count_event_end_time @email=@Mail, @service_name=@ServiceName, @start_time=@StartTime";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail, ServiceName = model.Title, StartTime = model.StartTime });
            var fullResult = string.Concat(result);
            if (fullResult != "Error")
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully calculated", Data = fullResult });
            else
                return NoContent();
        }
        // Post: api/Events/list-events
        [HttpPost]
        [Route("list-events")]
        public async Task<IActionResult> GetEvents([FromBody] EventDto model)
        {
            string sqlcmd = $"EXEC eo_events_select @email=@Mail";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail });
            var fullResult = string.Concat(result);

            if (fullResult != "Empty")
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully list services!", Data = fullResult });
            return NoContent();
        }

        // Post: api/Events/
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventDto model)
        {
            string sqlcmd = $"EXEC eo_events_insert @title=@Title, @date= @Date, @start_time= @StartTime, @end_time= @EndTime, @user_mail=@Mail";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Title = model.Title, Date = model.Date, StartTime = model.StartTime, EndTime = model.EndTime, Mail = model.UserMail });
            var fullResult = string.Concat(result);

            if (fullResult == "Success")
                return Ok(new ResponseDto { Status = "Success", Message = fullResult });
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = fullResult });
        }

        //PUT: api/Events/
        [HttpPut]
        public async Task<IActionResult> PutEvent([FromBody] EventDto model)
        {
            string sqlcmd = $"EXEC eo_events_update @title=@Title, @date= @Date, @start_time= @StartTime, @end_time= @EndTime, @user_mail=@Mail, @event_id={model.Id}";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Title = model.Title, Date = model.Date, StartTime = model.StartTime, EndTime = model.EndTime, Mail = model.UserMail });
            var fullResult = string.Concat(result);

            if (fullResult == "Success")
                return Ok(new ResponseDto { Status = "Success", Message = fullResult });
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = fullResult });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var _event = await _context.Events.FindAsync(id);
            if (_event == null)
            {
                return NotFound(new ResponseDto { Status = "Error", Message = "Event not found." });
            }

            _context.Events.Remove(_event);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [Route("upcoming")]
        public async Task<ActionResult<Event>> GetUpcomingEvent([FromBody] EventDto model)
        {
            string sqlcmd = "EXEC eo_get_upcoming_events @email=@Mail";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd, new { Mail = model.UserMail });
            var fullResult = string.Concat(result);
            if (fullResult != "")
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully calculated", Data = fullResult });
            return NoContent();
        }

    }
}
