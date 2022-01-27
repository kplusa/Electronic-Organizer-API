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

        [HttpPost]
        public IActionResult GetRecognition([FromBody] RecognitionDto model)
        {
            Console.WriteLine(model.RecognizedText);
            var result = EventRecognition.ShowAsJsonString(model.RecognizedText);
            Console.WriteLine(result);
            if (result != "Error")
                return Ok(new ResponseDto { Status = "Success", Message = "Successfully recognized", Data = result });
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "An error occurred while recognizing text" });
        }
    }
}