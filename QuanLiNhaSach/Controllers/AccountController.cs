using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLiNhaSach.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var vm = new UserViewModel() { Address = user.Address, DateOfBirth = user.DateOfBirth, Id = user.Id, Mail = user.Email, Username = user.UserName, Name = user.FullName, Phone = user.PhoneNumber };

            return View(vm);
        }
    }
}
