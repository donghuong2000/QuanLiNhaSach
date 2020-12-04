using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll()
        {
            var obj = _db.Categories.ToList();
            return Json(new {data= obj });
        }
        public IActionResult Upsert(string id)
        {
            Category category = new Category();
            if(id==null)
            {
                return View(category);
            }
            category = _db.Categories.FirstOrDefault(x => x.Id == id);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);
            
        }
        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == null)
                {
                    //tạo mới
                    var newCate = new Category() { Id = Guid.NewGuid().ToString(), Name = category.Name };
                    _db.Categories.Add(newCate);
                    _db.SaveChanges();
                    return RedirectToAction("index");
                }
                var cate = _db.Categories.FirstOrDefault(x => x.Id == category.Id);
                if(cate!=null)
                {
                    // tồn tại thì có nghĩa là update
                    cate.Name = category.Name;
                    _db.Categories.Update(cate);
                    _db.SaveChanges();
                    return RedirectToAction("index");
                }
                ModelState.AddModelError("", "Lỗi hệ thống");
            }
            return View(category);
        }
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                var cate = _db.Categories.Find(id);
                _db.Categories.Remove(cate);
                _db.SaveChanges();
                return Json(new { success = true, message = "đã xóa thành công" });
            }
            catch (Exception e)
            {

                return Json(new { success = false, message = e.Message });
            }
        }
    }
}
