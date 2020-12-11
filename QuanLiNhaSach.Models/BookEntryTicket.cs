using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BookEntryTicket
    {
        public BookEntryTicket()
        {
            BookEntryTicketDetail = new HashSet<BookEntryTicketDetail>();
        }

        public string Id { get; set; }
        public DateTime DateEntry { get; set; }

        public virtual ICollection<BookEntryTicketDetail> BookEntryTicketDetail { get; set; }
    }
}
