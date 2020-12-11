using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BookExistDetail
    {
        public string Id { get; set; }
        public string BookExistHeaderId { get; set; }
        public DateTime TimeRecord { get; set; }
        public int FirstExist { get; set; }
        public int IncurredExist { get; set; }
        public int LastExist { get; set; }

        public virtual BookExistHeader BookExistHeader { get; set; }
    }
}
