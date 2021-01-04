using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using System;
using System.Linq;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewBag.DoanhThu = _db.Bills.Select(x => x.TotalPrice).Sum().ToString("#,###") + " VND";
            ViewBag.No = _db.AppUsers.Select(x => x.new_last_debit).Sum().ToString("#,###") + " VND";


            var list = _userManager.GetUsersInRoleAsync("Customer").Result.ToList();


            var adminlist = _userManager.Users.Where(x => !list.Contains(x)).ToList();


            ViewBag.UserAdminList=new SelectList(adminlist, "Id", "UserName");
            ViewBag.UserList = new SelectList(list, "Id", "UserName");
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

        public IActionResult Statistical_Revenue(string id,DateTime? date)
        {
            var dt = _db.Bills.Include(x => x.Staff).ToList();

            
            // lấy Doanh thu theo tháng của 1 tháng nào đó
            if (id != null)
            {
                var labels = new string[12];
                var values = new float[12];
                var time = date == null ? DateTime.Now : date.GetValueOrDefault();
                var obj = _db.Bills.Where(x => x.StaffId == id).ToList();
                for (int i = 0; i < 12; i++)
                {
                    var month = time.AddMonths((12 - i) * -1).ToString("MM-yyyy");
                    labels[i] = month;
                    //lấy doanh thu tương ứng với tháng đó
                    var a = obj.Where(x => x.DateCreate.ToString("MM-yyyy") == month).ToList();
                    values[i] = a.Sum(x => x.TotalPrice);
                }
                return Json(new { labels, values });

            }
            // lấy doanh thu của tất cả
            else
            {
                
                    var obj = dt.GroupBy(X => X.Staff.UserName).Select(s => new
                    {
                        name = s.Key,
                        sum = s.Sum(x => x.TotalPrice)
                    });
                    var values = obj.Select(x => x.sum).ToArray();
                    var labels = obj.Select(x => x.name).ToArray();
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
