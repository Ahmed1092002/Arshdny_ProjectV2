using System;
using System.Collections.Generic;

namespace Arshdny_ProjectV2.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        public string Qualification { get; set; } = null!;
        public string Roles { get; set; } = null!;
        public int Permission { get; set; }

       
    }
}
