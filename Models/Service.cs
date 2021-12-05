﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Electronic_Organizer_API.Models
{
    public partial class Service
    {
        public Service()
        {
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int EstimatedTime { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}