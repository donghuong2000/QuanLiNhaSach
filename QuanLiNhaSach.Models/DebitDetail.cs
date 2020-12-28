using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class DebitDetail
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime TimeRecord { get; set; }
        public float FirstDebit { get; set; }
        public float IncurredDebit { get; set; }
        public float LastDebit { get; set; }

        
        public virtual AppUser ApplicationUser { get; set; }
    }
}
