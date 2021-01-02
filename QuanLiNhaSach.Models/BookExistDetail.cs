using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BookExistDetail
    {
        public string Id { get; set; }
        public string BookId { get; set; }
        public DateTime TimeRecord { get; set; }
        public int FirstExist { get; set; }
        public int IncurredExist { get; set; }
        public int LastExist { get; set; }


        public virtual Book Book { get; set; }
    } 

}
