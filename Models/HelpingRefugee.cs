using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arshdny_ProjectV2.Models
{
    public partial class HelpingRefugee
    {
        [Key]
        public int RequestID { get; set; }

        [ForeignKey("Refugee")]
        public int RefugeeID { get; set; }

        public string? Message { get; set; }

        public DateTime RequestDate { get; set; }

        public bool RequestStatus { get; set; }


    }
}
