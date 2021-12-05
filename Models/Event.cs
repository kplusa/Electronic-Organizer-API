using System;
using System.Collections.Generic;

#nullable disable

namespace Electronic_Organizer_API.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DayOfTimetableId { get; set; }
        public int? ServiceId { get; set; }

        public virtual DayOfTimetable DayOfTimetable { get; set; }
        public virtual Service Service { get; set; }
    }
}
