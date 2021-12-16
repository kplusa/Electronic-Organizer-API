using System;
using System.Collections.Generic;

namespace Electronic_Organizer_API.Models
{
    public partial class Timetable
    {
        public Timetable()
        {
            DayOfTimetables = new HashSet<DayOfTimetable>();
        }

        public int Id { get; set; }
        public int EndUserId { get; set; }

        public virtual EndUser EndUser { get; set; }
        public virtual ICollection<DayOfTimetable> DayOfTimetables { get; set; }
    }
}
