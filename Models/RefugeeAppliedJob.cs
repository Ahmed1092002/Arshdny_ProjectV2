using System;
using System.Collections.Generic;

namespace Arshdny_ProjectV2.Models
{
    public partial class RefugeeAppliedJob
    {
        public int RefugeeId { get; set; }
        public int JobId { get; set; }
        public DateTime ApplyDate { get; set; }
        public int JobStatus { get; set; }


    }
}
