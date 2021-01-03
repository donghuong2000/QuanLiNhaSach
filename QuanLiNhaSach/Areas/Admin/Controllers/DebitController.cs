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
    public class DebitController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _usermanager;
        public DebitController(ApplicationDbContext db, UserManager<AppUser> usermanager)
        {
            _db = db;
            _usermanager = usermanager;
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
        public IActionResult Create_List_Debit(string id) // hàm sẽ tạo debit detail từ new first debit, new incurred debit , new last debit của user
        {
            if (id == null)
                return Json(new { success = false, message = "Vui lòng chọn tháng cần báo cáo trước khi tạo báo cáo !" }); 
            var timenow = DateTime.Parse(id); // biến string nhập vào là 1 ngày
            var result = IsAvailable_DebitDetail_Has_Time(timenow);
            if (result == false)
                return Json(new { success = false, message = "Đã tạo báo cáo của tháng này rồi, vui lòng không tạo lại" });
            if((timenow - DateTime.Now).TotalDays > 0)
                return Json(new { success = false, message = "Thời gian hiện tại chưa tới tháng cần tạo báo cáo, vui lòng chọn lại" });
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result; // tạo 1 list customer role là khách hàng từ list user
            DebitDetail debitdetail = new DebitDetail();
            foreach (var item in customerlist)
            {
                if (item.new_last_debit != 0)
                {
                    
                    debitdetail.Id = Guid.NewGuid().ToString();
                    debitdetail.ApplicationUserId = item.Id;
                    debitdetail.TimeRecord = timenow;
                    debitdetail.FirstDebit = item.new_first_debit;
                    debitdetail.IncurredDebit = item.new_incurred_debit;
                    debitdetail.LastDebit = item.new_last_debit;
                    _db.DebitDetails.Add(debitdetail);
                    _db.SaveChanges();
                    item.new_first_debit = item.new_last_debit;
                    item.new_incurred_debit = 0;
                    item.new_last_debit = item.new_first_debit + item.new_incurred_debit;
                    _db.AppUsers.Update(item);
                    _db.SaveChanges();
                }
            }
            return Json(new { success = true ,message = ""}) ; 
            
        }
        public bool IsAvailable_DebitDetail_Has_Time(DateTime time)
        {
            var list_debit_detail = _db.DebitDetails.ToList();
            foreach(var item in list_debit_detail)
            {
                if(item.TimeRecord == time )
                {
                    return false; // không add được vì đã có debit detail ở tháng đang chọn
                }
            }
            return true; // được phép add
        }
        //DateTime timenow = DateTime.Parse(id);
        //var time_now = timenow.ToString("MM-yyyy");
        //var time_past = timenow.AddMonths(-1).ToString("MM-yyyy");
        //var list_debit_detail = _db.DebitDetails.ToList();
        //List<DebitDetail> list_debit_detail_last_month = new List<DebitDetail>();
        //DebitDetail debit_detail_has_time_now = null;
        //foreach (var item in list_debit_detail)
        //{
        //    if (item.TimeRecord.ToString("MM-yyyy") == time_now)
        //    {
        //        debit_detail_has_time_now = item;
        //        break;
        //    }
        //    else if (item.TimeRecord.ToString("MM-yyyy") == time_past)
        //        list_debit_detail_last_month.Add(item);
        //}
        //if (debit_detail_has_time_now != null) // nếu đã có debit detail của tháng hiện tại , thì  không thể tạo được nữa
        //    return Json(new { success = false });

        //DebitDetail newdebit = new DebitDetail();
        //foreach (var item in list_debit_detail_last_month)
        //{
        //    newdebit.Id = Guid.NewGuid().ToString();
        //    newdebit.ApplicationUserId = item.ApplicationUserId;
        //    newdebit.TimeRecord = DateTime.Parse(time_now); // tháng hiện tại
        //    newdebit.FirstDebit = item.LastDebit;
        //    newdebit.IncurredDebit = 0;
        //    newdebit.LastDebit = newdebit.FirstDebit + newdebit.IncurredDebit;
        //    _db.DebitDetails.Add(newdebit);
        //    _db.SaveChanges();
        //}
        //return Json(new { success = true });
    }

}
