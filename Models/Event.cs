using System;
using System.Collections.Generic;

namespace Electronic_Organizer_API.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? ServiceId { get; set; }
        public int? EndUserId { get; set; }

        public virtual EndUser EndUser { get; set; }
        public virtual Service Service { get; set; }
    }
}
