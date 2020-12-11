using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Models.ViewModels;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostEnvironment _hostEnvironment;
        public BookController(ApplicationDbContext db, IHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll()
        {
            var obj = _db.Books.Include(x => x.Category).Select(x => new
            {
                id = x.Id,
                imgUrl = x.ImgUrl,
                name = x.Name,
                author = x.Author,
                category = x.Category.Name,
                decription = x.Decription.Substring(0, 10)+"...",
                quantity = x.Quantity,
                datePublish = x.DatePublish.ToShortDateString()
            }).ToList();
            return Json(new { data = obj });
        }
        public IActionResult Upsert(string id)
        {
            // cho selectlist của view
            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");
            var vm = new BookViewModel();
            if(id == null)
            {
                return View(vm);
            }
            var book = _db.Books.FirstOrDefault(x => x.Id == id);
            if(book == null)
            {
                return NotFound();
            }
            vm.Id = book.Id;
            vm.Name = book.Name;
            vm.Author = book.Author;
            vm.CategoryId = book.CategoryId;
            vm.Price = book.Price;
            vm.Quantity = book.Quantity;
            vm.DatePublish = book.DatePublish;
            vm.Decription = book.Decription;
            vm.ImgUrl = book.ImgUrl;
            return View(vm);

        }
        [HttpPost]
        public IActionResult Upsert(BookViewModel vm)
        {
            // cho selectlist của view
            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {

                // start up ảnh
                string fileName = "";
                string folderName = "wwwroot\\Media\\";
                string webRootPath = _hostEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (vm.File != null)
                {
                    fileName = ContentDispositionHeaderValue.Parse(vm.File.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        vm.File.CopyTo(stream);

                    }
                    vm.ImgUrl = @"/Media/" + fileName;
                }
                // Endd up ảnh
                if (vm.Id == null)
                {
                    //tạo mới
                    var newBook = new Book() { Id = Guid.NewGuid().ToString(), Name = vm.Name,Author = vm.Author,CategoryId = vm.CategoryId,Quantity = vm.Quantity,DatePublish = vm.DatePublish,Decription = vm.Decription,ImgUrl = vm.ImgUrl };
                    _db.Books.Add(newBook);
                    _db.SaveChanges();
                    return RedirectToAction("index");
                }
                var book = _db.Books.FirstOrDefault(x => x.Id == vm.Id);
                if (book != null)
                {
                    // tồn tại thì có nghĩa là update
                    book.Name = vm.Name;
                    book.Author = vm.Author;
                    book.CategoryId = vm.CategoryId;
                    book.DatePublish = vm.DatePublish;
                    book.Price = vm.Price;
                    book.Decription = vm.Decription;
                    book.Quantity = vm.Quantity;
                    if (vm.File!=null)
                    {
                        book.ImgUrl = vm.ImgUrl;
                    }    
                    _db.Books.Update(book);
                    _db.SaveChanges();
                    return RedirectToAction("index");
                }
                ModelState.AddModelError("", "Lỗi hệ thống");
            }
            return View(vm);
        }
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                var book = _db.Books.Find(id);
                _db.Books.Remove(book);
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
