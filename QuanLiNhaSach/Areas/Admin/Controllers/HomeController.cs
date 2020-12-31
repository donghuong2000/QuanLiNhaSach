using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }
        // thống kê nợ khách hàng
        public IActionResult Statistical_DeptCustomer(string id)
        {
            // lấy nợ của tất cả
            if (id == null)
            {

                var obj = _db.AppUsers.ToList();
                var values = obj.Select(x => x.Dept).ToArray();
                var labels = obj.Select(x => x.UserName).ToArray();

                return Json(new { labels, values });
            }
            // lấy nợ theo tháng của 1 thàng nào đó
            else
            {
                var labels = new string[12];
                var values = new float[12];
                var time = DateTime.Today;
                // lấy 12 tháng kể từ ngày hiện tại
                for (int i = 0; i < 12; i++)
                {
                    var month = time.AddMonths(i * -1).Month;
                    labels[i] = month.ToString();
                    //lấy cái nợ tương ứng với tháng đó
                    values[i] = _db.DebitDetails.Where(x => x.Id == id && x.TimeRecord.Month == month).Sum(x => x.LastDebit);
                }
                return Json(new { labels, values });
            }    
            
        }




    }

}
