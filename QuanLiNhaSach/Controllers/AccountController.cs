using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
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
        private readonly ApplicationDbContext _db;
        public AccountController(UserManager<AppUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            
            var user = await _userManager.GetUserAsync(User);
            ViewBag.dept = user.new_last_debit.ToString("#,###")+" VND";
            var vm = new UserViewModel() { Address = user.Address, DateOfBirth = user.DateOfBirth, Id = user.Id, Mail = user.Email, Username = user.UserName, Name = user.FullName, Phone = user.PhoneNumber };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                // id null là thằng mới
               
                    var user = await _userManager.FindByIdAsync(vm.Id);
                    user.FullName = vm.Name;
                    user.Address = vm.Address;
                    user.DateOfBirth = vm.DateOfBirth;
                    user.PhoneNumber = vm.Phone;
                    user.Email = vm.Mail;
                    user.NormalizedEmail = vm.Mail.ToUpper();

                    // thay đổi role
                    var oldRoles = await _userManager.GetRolesAsync(user);

                    await _userManager.RemoveFromRolesAsync(user, oldRoles);

                    await _userManager.AddToRolesAsync(user, vm.Roles);
                    // end thay đổi role

                    //update thông tin
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    ViewBag.dept = user.new_last_debit.ToString("#,###") + " VND";

            }
            return View(vm);
        }
        public async Task<IActionResult> Changepassword(string cur,string np,string cnp)
        {
            if(cur == null || np == null || cnp == null)
            {
                return Json(new { success = false, message = "Không được để trống các ô" });
            }    
            if(np != cnp)
            {
                return Json(new { success = false, message = "Xác nhận mật khẩu không chính xác" });
            }
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, cur, np);
                if(result.Succeeded)
                {
                    return Json(new { success = true, message = "thay đổi mật khẩu thành công" });
                }
                string error = "";
                foreach (var item in result.Errors)
                {
                    error += item.Description +"\n";
                }
                return Json(new { success = false, message = error });
            }
            catch (Exception e )
            {

                return Json(new { success = false, message = e.Message });
            }
        }

        public async Task<IActionResult> GetDebit()
        {
            var user = await _userManager.GetUserAsync(User);
            var obj = _db.DebitDetails
                .Include(x => x.ApplicationUser)
                .Where(x=>x.ApplicationUserId == user.Id)
                .Select(x => new
                {
                    id = x.Id,
                    customer = x.ApplicationUser.FullName,
                    date = x.TimeRecord.ToString("MM-yyyy"),
                    firstdebit = x.FirstDebit,
                    incurreddebit = x.IncurredDebit,
                    lastdebit = x.LastDebit,
                }).ToList();


            
            return Json(new { data = obj });
        }
        public async Task<IActionResult> GetHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var obj = _db.Bills
                .Include(x => x.ApplicationUser)
                .Include(x => x.Staff)
                .Where(x=>x.ApplicationUserId == user.Id)
                .Select(x => new
                {
                    id = x.Id,
                    staff = x.Staff.FullName,
                    customer = x.ApplicationUser.FullName,
                    date = x.DateCreate.ToShortDateString(),
                    total = x.TotalPrice
                });
            return Json(new { data = obj });



            
        }
        public IActionResult BillDetail(string id)
        {
            var bill = _db.Bills
                .Include(x => x.ApplicationUser)
                .Include(x => x.Staff)
                .FirstOrDefault(x => x.Id == id);

            if (bill == null)
            {
                return NotFound();
            }
            var billDetails = _db.BillDetails.Include(x => x.Book).Where(x => x.BillId == id).Select(x => x).ToList();

            bill.BillDetail = billDetails;
            return View(bill);

        }
    }
}
