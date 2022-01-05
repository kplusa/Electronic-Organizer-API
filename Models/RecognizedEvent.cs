using System;

namespace Electronic_Organizer_API.Models
{
    public class RecognizedEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Date { get; set; }
    }
}
