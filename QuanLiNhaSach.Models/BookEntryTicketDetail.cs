using System;
using System.Collections.Generic;

namespace QuanLiNhaSach.Models
{
    public partial class BookEntryTicketDetail
    {
        public string BookEntryTicketId { get; set; }
        public string BookId { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Author { get; set; }
        public int Count { get; set; }

        public virtual Book Book { get; set; }
        public virtual BookEntryTicket BookEntryTicket { get; set; }
        public virtual Category Category { get; set; }
    }
}
