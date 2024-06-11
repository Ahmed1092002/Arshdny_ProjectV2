using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arshdny_ProjectV2.Models
{
    public partial class Feedback
    {
        public int FeedbackID { get; set; }

        [ForeignKey("Refugee")]
        public int RefugeeID { get; set; }

        public int Rating { get; set; }

        public string? Message { get; set; }

        public DateTime FeedbackDate { get; set; }


    }
}
