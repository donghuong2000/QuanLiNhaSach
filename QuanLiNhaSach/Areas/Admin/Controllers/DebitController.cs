using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DebitController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DebitController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll(string id)
        {
            var obj = _db.DebitDetails
                .Include(x => x.ApplicationUser).Select(x => new
                {
                    id = x.Id,
                    customer = x.ApplicationUser.FullName,
                    date = x.TimeRecord.ToString("MM-yyyy"),
                    firstdebit = x.FirstDebit,
                    incurreddebit = x.IncurredDebit,
                    lastdebit = x.LastDebit,
                }).ToList();
          ;
             
            if (id != null)
            {
                var time = DateTime.Parse(id).ToString("MM-yyyy");
                var debit = obj.Where(x => x.date == time).ToList();
                return Json(new { data = debit });
            }
            return Json(new { data = obj });
        }
        public IActionResult Update_List_Debit(string id) // hàm này sẽ tự động tạo debit detail từ các debit detail có của tháng trước
        {
            DateTime timenow = DateTime.Parse(id);
            var time_now = timenow.ToString("MM-yyyy");
            var time_past = timenow.AddMonths(-1).ToString("MM-yyyy");
            var list_debit_detail = _db.DebitDetails.ToList();
            List<DebitDetail> list_debit_detail_last_month = new List<DebitDetail>();
            DebitDetail debit_detail_has_time_now = null;
            foreach (var item in list_debit_detail)
            {
                if (item.TimeRecord.ToString("MM-yyyy") == time_now)
                {
                    debit_detail_has_time_now = item;
                    break;
                }
                else if (item.TimeRecord.ToString("MM-yyyy") == time_past)
                    list_debit_detail_last_month.Add(item);
            }
            if (debit_detail_has_time_now != null) // nếu đã có debit detail của tháng hiện tại , thì  không thể tạo được nữa
                return Json(new { success = false });
            
            DebitDetail newdebit = new DebitDetail();
            foreach (var item in list_debit_detail_last_month)
            {
                newdebit.Id = Guid.NewGuid().ToString();
                newdebit.ApplicationUserId = item.ApplicationUserId;
                newdebit.TimeRecord = DateTime.Parse(time_now); // tháng hiện tại
                newdebit.FirstDebit = item.LastDebit;
                newdebit.IncurredDebit = 0;
                newdebit.LastDebit = newdebit.FirstDebit + newdebit.IncurredDebit;
                _db.DebitDetails.Add(newdebit);
                _db.SaveChanges();
            }
            return Json(new { success = true });
        }
    }
     
}
