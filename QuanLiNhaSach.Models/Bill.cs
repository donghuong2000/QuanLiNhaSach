using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetail = new HashSet<BillDetail>();
        }

        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string StaffId { get; set; }
        public DateTime DateCreate { get; set; }
        public float TotalPrice { get; set; }

        public virtual AppUser ApplicationUser { get; set; }
        public virtual AppUser Staff { get; set; }
        public virtual ICollection<BillDetail> BillDetail { get; set; }
    }
}
