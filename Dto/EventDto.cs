using System;

namespace Electronic_Organizer_API.Dto
{
    public class EventDto
    {
        public string UserMail { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
