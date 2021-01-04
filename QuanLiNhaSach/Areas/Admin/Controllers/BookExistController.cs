using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Roles = "Admin,Manager")]
    public class BookExistController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BookExistController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll(string id)
        {
            var obj = _db.BookExistDetails
                .Select(x => new
                {
                    id = x.Id,
                    book = x.Book.Name,
                    date = x.TimeRecord.ToString("MM-yyyy"),
                    firstexist = x.FirstExist,
                    incurredexist = x.IncurredExist,
                    lastexist = x.LastExist,
                }).ToList();
            ;

            if (id != null)
            {
                var time = DateTime.Parse(id).ToString("MM-yyyy");
                var exist = obj.Where(x => x.date == time).ToList();
                return Json(new { data = exist });
            }
            return Json(new { data = obj });
        }

        // báo cáo tồn sách theo tháng

        public IActionResult ExistReport()
        {
            var timenow = DateTime.Parse(DateTime.Now.ToString("MM-yyyy"));
            var result = Reported(timenow);
            if (result == false)
                return Json(new { success = false, message = "Đã tạo báo cáo của tháng này rồi, vui lòng không tạo lại" });

            var booklist = _db.Books.ToList();
            BookExistDetail exist = new BookExistDetail();

            foreach (var book in booklist)
            {
                if (book.Quantity != 0 || (book.new_first_exist + book.new_incurred_exist ==0 && book.new_first_exist !=0)) // khác 0 nghĩ là ít nhất sách đã được nhập
                {
                    // đưa vào exist detail để báo cáo
                    exist.FirstExist = book.new_first_exist;
                    exist.IncurredExist = book.new_incurred_exist;
                    exist.LastExist = exist.FirstExist + exist.IncurredExist;
                    exist.Id = Guid.NewGuid().ToString();
                    exist.BookId = book.Id;
                    exist.TimeRecord = timenow;
                    _db.BookExistDetails.Add(exist);
                    _db.SaveChanges();


                    // lưu cũ để update
                    book.old_incurred_exist = book.new_incurred_exist;
                    book.old_first_exist = book.new_first_exist;
                    book.old_Quantity = book.Quantity; 
                    _db.Books.Update(book);
                    _db.SaveChanges();


                    // set lại ban đầu 
                    book.new_first_exist = book.Quantity;
                    book.new_incurred_exist = 0;
                    book.Quantity = book.new_first_exist + book.new_incurred_exist;
                    _db.Books.Update(book);
                    _db.SaveChanges();

                }
            }

            return Json(new { success = true, message = "" });

        }

        public IActionResult Update_Report()
        {
            var timenow = DateTime.Parse(DateTime.Now.ToString("MM-yyyy"));
            var result = Reported(timenow);
            if (result == true)
                return Json(new { success = false, message = "Chưa tạo báo cáo của tháng hiện tại, nên không thể cập nhật báo cáo được" });
            var booklist = _db.Books.ToList();
            BookExistDetail exist = new BookExistDetail();
            // xóa báo cáo cũ 
            Remove_oldReport(timenow);
            foreach (var book in booklist)
            {
                if (book.old_Quantity != 0 || (book.old_first_exist + book.old_incurred_exist == 0 && book.old_first_exist !=0)) 
                {
                    // lưu cũ để update
                    book.old_incurred_exist = book.old_incurred_exist + book.new_incurred_exist;
                    book.old_Quantity = book.old_first_exist + book.old_incurred_exist;
                    _db.Books.Update(book);
                    _db.SaveChanges();

                    // update
                    exist.FirstExist = book.old_first_exist;
                    exist.IncurredExist = book.old_incurred_exist;
                    exist.LastExist = book.old_Quantity; 
                    exist.Id = Guid.NewGuid().ToString();
                    exist.BookId = book.Id;
                    exist.TimeRecord = timenow;
                    _db.BookExistDetails.Add(exist);
                    _db.SaveChanges();



                    // set lại ban đầu 
                    book.new_first_exist = book.Quantity;
                    book.new_incurred_exist = 0;
                    book.Quantity = book.new_first_exist;
                    _db.Books.Update(book);
                    _db.SaveChanges();

                }
                else if (book.Quantity != 0 || ((book.new_first_exist + book.new_incurred_exist) == 0 && book.new_first_exist != 0)) // nếu nợ cũ không tồn tại và nợ mới thì có -> phải tạo mới
                {

                    exist.FirstExist = book.new_first_exist;
                    exist.IncurredExist = book.new_incurred_exist;
                    exist.LastExist = book.Quantity;
                    exist.Id = Guid.NewGuid().ToString();
                    exist.BookId = book.Id;
                    exist.TimeRecord = timenow;
                    _db.BookExistDetails.Add(exist);
                    _db.SaveChanges();
                    // lưu cũ để update
                    book.old_incurred_exist = book.new_incurred_exist;
                    book.old_first_exist = book.new_first_exist;
                    book.old_Quantity = book.Quantity;
                    _db.Books.Update(book);
                    _db.SaveChanges();


                    // set lại ban đầu 
                    book.new_first_exist = book.Quantity;
                    book.new_incurred_exist = 0;
                    book.Quantity = book.new_first_exist + book.new_incurred_exist;
                    _db.Books.Update(book);
                    _db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "" });
        }
        public void Remove_oldReport(DateTime time) // xóa các exist detail có tháng ghi là tháng đầu vào
        {
            var list_exist_detail = _db.BookExistDetails.ToList();
            foreach (var item in list_exist_detail)
            {
                if (item.TimeRecord == time)
                {
                    _db.BookExistDetails.Remove(item);
                    _db.SaveChanges();
                }
            }
        }


        public bool Reported(DateTime time)
        {
            var list_exist_detail = _db.BookExistDetails.ToList();
            foreach (var item in list_exist_detail)
            {
                if (item.TimeRecord == time)
                {
                    return false; // 
                }
            }
            return true; // được phép add
        }
    }

}