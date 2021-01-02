using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public partial class Receipt
    {

        [Required]
        public string Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public DateTime DateCreate { get; set; }
        [Required]
        public float Proceeds { get; set; }

        public virtual AppUser ApplicationUser { get; set; }
    }
}
