using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BillDetail
    {
        public string BillId { get; set; }
        public string BookId { get; set; }
        public int? Count { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual Book Book { get; set; }
    }
}
