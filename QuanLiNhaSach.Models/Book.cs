using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public partial class Book
    {
        public Book()
        {
            BillDetail = new HashSet<BillDetail>();
            BookEntryTicketDetail = new HashSet<BookEntryTicketDetail>();
            BookExistDetail = new HashSet<BookExistDetail>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImgUrl { get; set; }
        public string CategoryId { get; set; }
        public DateTime DatePublish { get; set; }
        public string Decription { get; set; }
        public float Price { get; set; }
        //public int Quantity { get; set; }
        
        //public float old_first_exist { get; set; }
        //public float old_incurred_exist { get; set; }

        //public float old_last_exist { get; set; }

        public int new_first_exist { get; set; }
        public int new_incurred_exist { get; set; }
        public int quantity { get; set; } // new_last_exist




        public virtual Category Category { get; set; }
        public virtual ICollection<BillDetail> BillDetail { get; set; }
        public virtual ICollection<BookEntryTicketDetail> BookEntryTicketDetail { get; set; }
        public virtual ICollection<BookExistDetail> BookExistDetail { get; set; }

        public object include()
        {
            throw new NotImplementedException();
        }
    }
}
