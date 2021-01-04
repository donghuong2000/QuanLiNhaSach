using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public IActionResult GetAll(string mth)
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
              
            if (mth != null)
            {
                var time = DateTime.Parse(mth).ToString("MM-yyyy");
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

            foreach( var book in booklist)
            {
                if (book.Quantity != 0) // khác 0 nghĩ là ít nhất sách đã được nhập
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
                    _db.Books.Update(book);
                    _db.SaveChanges(); 

                    // set lại ban đầu 
                    book.new_incurred_exist = 0;


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


            foreach (var book in booklist)
            {
                if(book.new_incurred_exist !=0)
                {
                    Remove_oldReport(book.Id);

                    // lưu cũ để update
                    book.old_incurred_exist = book.new_incurred_exist;
                    _db.Books.Update(book);
                    _db.SaveChanges();

                    // update
                    exist.FirstExist = book.new_first_exist;
                    exist.IncurredExist = book.new_incurred_exist + book.old_incurred_exist;
                    exist.LastExist = exist.FirstExist + exist.IncurredExist;
                    exist.Id = Guid.NewGuid().ToString();
                    exist.BookId = book.Id;
                    exist.TimeRecord = timenow;
                    _db.BookExistDetails.Add(exist);
                    _db.SaveChanges();



                    // set lại ban đầu 
                    book.new_incurred_exist = 0;


                }
            }
            return Json(new { success = true, message = "" });
        }
        public void Remove_oldReport(string id) // xóa các exist detail có tháng ghi là tháng đầu vào
        {
            var list_exist_detail = _db.BookExistDetails.ToList();
            foreach (var item in list_exist_detail)
            {
                if (item.BookId == id)
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
