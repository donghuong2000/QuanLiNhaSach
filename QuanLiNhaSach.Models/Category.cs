using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public partial class Category
    {
        public Category()
        {
            Books = new HashSet<Book>();
            BookEntryTicketDetail = new HashSet<BookEntryTicketDetail>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<BookEntryTicketDetail> BookEntryTicketDetail { get; set; }
    }
}
