using Dapper;
using Electronic_Organizer_API.Dto;
using Electronic_Organizer_API.Models;
using Electronic_Organizer_API.Recognition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Electronic_Organizer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecognitionController : ControllerBase
    {
        private readonly ElectronicOrganizerContext _context;

        public RecognitionController(ElectronicOrganizerContext context)
        {
            _context = context;
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet]
        public async Task<IActionResult> GetRecognitionn()
        {
            string sqlcmd = "exec eo_recignized_events_select";
            using SqlConnection connection = new(_context.Database.GetConnectionString());
            var result = await connection.QueryAsync<string>(sqlcmd);
            var fullResult = string.Concat(result);
            dynamic json = JObject.Parse(fullResult);
            //foreach (var e in json)
            //{
            //    Console.WriteLine(e);
 
            //}
            Console.WriteLine(json.responses[0].fullTextAnnotation.text);
            return Ok(new ResponseDto { Status = "Success", Message = "Successfully recognized", Data= json.responses[0].fullTextAnnotation.text });
        }

        [HttpPost]
        public IActionResult GetRecognition([FromBody] RecognitionDto model)
        {
            //string sqlcmd = "exec eo_recignized_events_select";
            //using SqlConnection connection = new(_context.Database.GetConnectionString());
            //var result = await connection.QueryAsync<string>(sqlcmd);
            //var fullResult = string.Concat(result);
            //dynamic json = JObject.Parse(fullResult);
            //foreach (var e in json)
            //{
            //    Console.WriteLine(e);

            //}
            //Console.WriteLine(json.responses[0].fullTextAnnotation.text);
            Console.WriteLine(model.RecognizedText);
            var result = EventRecognition.ShowAsJsonString(model.RecognizedText);
            Console.WriteLine(result);
            return Ok(new ResponseDto { Status = "Success", Message = "Successfully recognized", Data = result });
        }
    }
}