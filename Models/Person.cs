using System;
using System.Collections.Generic;

namespace Arshdny_ProjectV2.Models
{
    public partial class Person
    {
        

        public int PersonId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string? Phone2 { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = null!;

    }
}
