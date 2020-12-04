using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public class Category
    {
        public Category()
        {
            Books = new HashSet<Book>();
        }
        public string Id{ get; set; }
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
