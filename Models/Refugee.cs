using System;
using System.Collections.Generic;

namespace Arshdny_ProjectV2.Models
{
    public partial class Refugee
    {
        

        public int RefugeeId { get; set; }
        public int RefugeeJobId { get; set; }
        public int UserId { get; set; }
        public string RefugeeCardNo { get; set; } = null!;
        public int CountryId { get; set; }
        public int NationaltyId { get; set; }
        public string? Cv { get; set; }
        public string ImagePath { get; set; } = null!;
        public DateTime CardStartDate { get; set; }
        public DateTime CardEndDate { get; set; }

        public string DeviceToken { get; set; }

        
    }
}
