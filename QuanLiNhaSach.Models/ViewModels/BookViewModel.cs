using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLiNhaSach.Models.ViewModels
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            DatePublish = DateTime.Now;
        }
        public string Id { get; set; }
        [Required]
        [Display(Name="Tên Sách")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Tên Tác giả")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Giá tiền")]
        public float Price { get; set; }
        [Required]
        [Display(Name = "Danh mục")]
        public string CategoryId { get; set; }
        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày xuất bản")]
        public DateTime DatePublish { get; set; }
        [Required]
        [Display(Name = "Mô tả")]
        public string Decription { get; set; }
        
        [Display(Name = "Ảnh bìa")]
        public IFormFile File { get; set; }
        public string ImgUrl { get; set; }

        
    }
}
