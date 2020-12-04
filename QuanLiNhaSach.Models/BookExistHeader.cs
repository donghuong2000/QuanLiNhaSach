using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BookExistHeader
    {
        public BookExistHeader()
        {
            BookExistDetail = new HashSet<BookExistDetail>();
        }

        public string Id { get; set; }
        public string BookId { get; set; }
        public int? TotalExist { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<BookExistDetail> BookExistDetail { get; set; }
    }
}
