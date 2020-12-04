using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public class AppUser : IdentityUser
    {

        public string FullName { get; set; }

        public string  Address { get; set; }

        public DateTime DateOfBirth { get; set; }



    }
}
