using System;
using System.Collections.Generic;

namespace Electronic_Organizer_API.Models
{
    public partial class DayOfTimetable
    {
        public DayOfTimetable()
        {
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public int DayId { get; set; }
        public int TimetableId { get; set; }

        public virtual Day Day { get; set; }
        public virtual Timetable Timetable { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
