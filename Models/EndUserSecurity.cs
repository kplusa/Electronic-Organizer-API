using System;
using System.Collections.Generic;

namespace Electronic_Organizer_API.Models
{
    public partial class EndUserSecurity
    {
        public int Id { get; set; }
        public string Salt { get; set; }
        public string HashedPassword { get; set; }
        public int EndUserId { get; set; }

        public virtual EndUser EndUser { get; set; }
    }
}
