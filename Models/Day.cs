using System;
using System.Collections.Generic;

#nullable disable

namespace Electronic_Organizer_API.Models
{
    public partial class Day
    {
        public Day()
        {
            DayOfTimetables = new HashSet<DayOfTimetable>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<DayOfTimetable> DayOfTimetables { get; set; }
    }
}
