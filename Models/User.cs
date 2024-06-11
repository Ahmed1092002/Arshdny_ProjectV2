using System;
using System.Collections.Generic;

namespace Arshdny_ProjectV2.Models
{
    public partial class User
    {
       

        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; } 


    }
}
