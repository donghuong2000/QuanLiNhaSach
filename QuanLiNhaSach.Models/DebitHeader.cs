using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class DebitHeader
    {
        public DebitHeader()
        {
            DebitDetail = new HashSet<DebitDetail>();
        }

        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public float TotalDebit { get; set; }

        public virtual AppUser ApplicationUser { get; set; }
        public virtual ICollection<DebitDetail> DebitDetail { get; set; }
    }
}
