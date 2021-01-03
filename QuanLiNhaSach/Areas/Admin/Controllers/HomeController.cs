using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using System;
using System.Linq;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.UserList = new SelectList(_db.AppUsers.ToList(), "Id", "UserName");
            return View();
        }
        // thống kê nợ khách hàng
        public IActionResult Statistical_DeptCustomer(string id,DateTime? date)
        {
           
            // lấy nợ theo tháng của 1 thàng nào đó
            if (id != null && date !=null )
            {
                var labels = new string[12];
                var values = new float[12];
                var time = date.GetValueOrDefault();
                // lấy nợ của user
                var deptUser = _db.DebitDetails.Where(x => x.ApplicationUserId == id).ToList();
                // lấy 12 tháng kể từ ngày hiện tại
                for (int i = 0; i < 12; i++)
                {
                    var month = time.AddMonths((12 - i) * -1).ToString("MM-yyyy");
                    labels[i] = month;
                    //lấy cái nợ tương ứng với tháng đó
                    values[i] = deptUser.Where(x => x.TimeRecord.ToString("MM-yyyy") == month).Sum(x => x.LastDebit);
                }
                return Json(new { labels, values });
                
            }
            // lấy nợ của tất cả
            else
            {
                var obj = _db.AppUsers.ToList();
                var values = obj.Select(x => x.new_last_debit).ToArray();
                var labels = obj.Select(x => x.UserName).ToArray();

                return Json(new { labels, values });
            }    
            
        }


        public IActionResult Statistical_Book()
        {
            var obj = _db.BillDetails.Include(x => x.Book)
                .GroupBy(x => x.Book.Name)
                .Select(x => new
                {
                    book = x.Key,
                    count = x.Sum(x => x.Count)

                }).OrderByDescending(x=>x.count).ToList();
            var labels = obj.Select(x => x.book).ToArray();
            var values = obj.Select(x => x.count).ToArray();

            return Json(new { labels, values });
            
        }

    }

}
