using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(string id)
        {
            if(id==null)
            {
                return View(new IdentityRole());
            }
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(IdentityRole role)
        {
            var oldRole = await _roleManager.FindByIdAsync(role.Id);
            if (oldRole != null)
            {
                oldRole.Name = role.Name;
                await _roleManager.UpdateAsync(oldRole);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.Name));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(role);

        }
        public IActionResult GetAll()
        {
            var obj = _roleManager.Roles.ToList();

            return Json(new {data = obj });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _roleManager.FindByIdAsync(id);
                await _roleManager.DeleteAsync(user);
                return Json(new { success = true, message = "đã xóa thành công người dùng" });

            }
            catch (Exception ex)
            {

                return Json(new { success = true, message = ex.Message });

            }
        }

    }
}
