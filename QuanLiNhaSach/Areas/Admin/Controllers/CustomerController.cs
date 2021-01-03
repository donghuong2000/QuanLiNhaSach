using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Manager,Admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public CustomerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Upsert(string id)
        {
            // truyền role list cho view
            


            var vm = new UserViewModel() { DateOfBirth = DateTime.Now };
            if (id == null)
            {
                return View(vm);
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            vm.Id = user.Id;
            vm.Username = user.UserName;
            vm.Mail = user.Email;
            vm.Name = user.FullName;
            vm.Address = user.Address;
            vm.DateOfBirth = user.DateOfBirth;
            vm.Phone = user.PhoneNumber;
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(UserViewModel vm)
        {
           
            
            if (ModelState.IsValid)
            {
                // id null là thằng mới
                if (vm.Id == null)
                {
                    try
                    {
                        var user = new AppUser() { UserName = vm.Username, FullName = vm.Name, Email = vm.Mail, PhoneNumber = vm.Phone, Address = vm.Address, DateOfBirth = vm.DateOfBirth };
                        var result = await _userManager.CreateAsync(user, vm.Password);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRolesAsync(user, vm.Roles);
                            return RedirectToAction("Index");
                        }
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError("", ex.Message);
                    }

                }
                else
                {
                    var user = await _userManager.FindByIdAsync(vm.Id);
                    user.FullName = vm.Name;
                    user.Address = vm.Address;
                    user.DateOfBirth = vm.DateOfBirth;
                    user.PhoneNumber = vm.Phone;
                    user.Email = vm.Mail;
                    user.NormalizedEmail = vm.Mail.ToUpper();
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(vm);

        }
        private static bool isLock(DateTimeOffset date)
        {
            TimeSpan time = date.DateTime - DateTime.Now;
            if (time.TotalDays < 0)
            {
                return false;
            }
            return true;
        }


        public IActionResult GetAll()
        {

            var obj = _userManager.Users.Select(x => new
            {
                id = x.Id,
                username = x.UserName,
                name = x.FullName,
                email = x.Email,
                role = _userManager.GetRolesAsync(x).Result,
                islock = isLock(x.LockoutEnd.GetValueOrDefault()),
            }).ToList();
            obj = obj.Where(x => !x.role.Contains("Admin") && !x.role.Contains("Manager")).ToList();
            return Json(new { data = obj });


        }
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                // true là đang khóa
                if (isLock(user.LockoutEnd.GetValueOrDefault()))
                {

                    user.LockoutEnd = DateTimeOffset.Now;
                }
                else
                {
                    user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
                }
                await _userManager.UpdateAsync(user);
               
                return Json(new { success = true, message = "Đã khóa/mở khóa người dùng thành công" });
            }
            return Json(new { success = false, message = "Lỗi hệ thống" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);
                return Json(new { success = true, message = "đã xóa thành công người dùng" });

            }
            catch (Exception ex)
            {

                return Json(new { success = true, message = ex.Message });

            }
        }
    }
}
