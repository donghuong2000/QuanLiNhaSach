using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Models.ViewModels;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext db, UserManager<AppUser> usermanager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _usermanager = usermanager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Upsert(string id)
        {
            // truyền role list cho view
            ViewBag.RoleList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");


            var vm = new UserViewModel() { DateOfBirth = DateTime.Now };
            if (id == null)
            {
                return View(vm);
            }
            var user = _db.AppUsers.FirstOrDefault(x => x.Id == id);
            if(user == null)
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
            vm.Roles = _usermanager.GetRolesAsync(user).Result.ToList();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(UserViewModel vm)
        {
            // truyền role list cho view
            ViewBag.RoleList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            if (ModelState.IsValid)
            {
                // id null là thằng mới
                if (vm.Id == null)
                {
                    try
                    {
                        var user = new AppUser() { UserName = vm.Username, FullName = vm.Name, Email = vm.Mail, PhoneNumber = vm.Phone, Address = vm.Address, DateOfBirth = vm.DateOfBirth };
                        var result = await _usermanager.CreateAsync(user, vm.Password);
                        if (result.Succeeded)
                        {
                            await _usermanager.AddToRolesAsync(user, vm.Roles);
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
                    var user = await _usermanager.FindByIdAsync(vm.Id);
                    user.FullName = vm.Name;
                    user.Address = vm.Address;
                    user.DateOfBirth = vm.DateOfBirth;
                    user.PhoneNumber = vm.Phone;
                    user.Email = vm.Mail;
                    user.NormalizedEmail = vm.Mail.ToUpper();

                    // thay đổi role
                    var oldRoles = await _usermanager.GetRolesAsync(user);

                    await _usermanager.RemoveFromRolesAsync(user, oldRoles);

                    await _usermanager.AddToRolesAsync(user, vm.Roles);
                    // end thay đổi role

                    //update thông tin
                    var result = await _usermanager.UpdateAsync(user);
                    if(result.Succeeded)
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
            if( time.TotalDays < 0)
            {
                return false;
            }
            return true;
        }
        public IActionResult Test()
        {
            var obj = 1;
            return Json(new { data = obj });
        }
        
        public IActionResult GetAll()
        {
           
                var obj = _db.AppUsers.Select(x => new
                {
                    id = x.Id,
                    username = x.UserName,
                    name = x.FullName,
                    email = x.Email,
                    role = _usermanager.GetRolesAsync(x).Result,
                    islock = isLock(x.LockoutEnd.GetValueOrDefault()),
                }).ToList();
              
            return Json(new { data = obj });
           
               
        }
        public IActionResult LockUnLock(string id)
        {
            var user = _db.AppUsers.FirstOrDefault(x => x.Id == id);
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
                _db.AppUsers.Update(user);
                _db.SaveChanges();
                return Json(new { success = true, message = "Đã khóa/mở khóa người dùng thành công" });
            }
            return Json(new { success = false, message = "Lỗi hệ thống" });
        }
        [HttpDelete]
        public async Task< IActionResult> Delete(string id)
        {
            try
            {
                var user = await _usermanager.FindByIdAsync(id);
                await _usermanager.DeleteAsync(user);
                return Json(new { success = true, message = "đã xóa thành công người dùng" });
                 
            }
            catch (Exception ex)
            {

                return Json(new { success = true, message = ex.Message });

            }
        }
    }
}
