using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class Receipt
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime? DateCreate { get; set; }
        public float? Proceeds { get; set; }
    }
}
