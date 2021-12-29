using System;
using System.Collections.Generic;

namespace Electronic_Organizer_API.Models
{
    public partial class EndUser
    {
        public EndUser()
        {
            EndUserSecurities = new HashSet<EndUserSecurity>();
            Events = new HashSet<Event>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<EndUserSecurity> EndUserSecurities { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
