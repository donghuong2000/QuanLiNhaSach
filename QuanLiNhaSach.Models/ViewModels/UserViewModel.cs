using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLiNhaSach.Models.ViewModels
{
    public class UserViewModel
    {

        public UserViewModel()
        {
            Roles = new List<string>();
        }
        [Display(Name="Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Tên tài khoản")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Mail { get; set; }
        [Required]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password",ErrorMessage ="Mật khẩu xác nhận không chính xác")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Chức vụ")]
        [Required]
        public List<string> Roles { get; set; }


    }
}
