using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public class Book
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImgUrl { get; set; }

        public string Decription { get; set; }

        public int Quantity { get; set; }
        public string Author { get; set; }

        public DateTime DatePublish { get; set; }

        public  string CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

    }
}
