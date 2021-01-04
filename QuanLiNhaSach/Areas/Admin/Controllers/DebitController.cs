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
          
             
            if (id != null)
            {
                var time = DateTime.Parse(id).ToString("MM-yyyy");
                var debit = obj.Where(x => x.date == time).ToList();
                return Json(new { data = debit });
            }
            return Json(new { data = obj });
        }
        public IActionResult Create_List_Debit() // hàm sẽ tạo debit detail từ new first debit, new incurred debit , new last debit của user
        {
            var timenow = DateTime.Parse(DateTime.Now.ToString("MM-yyyy")); // biến string nhập vào là 1 ngày
            var result = IsAvailable_DebitDetail_Has_Time(timenow);
            if (result == false)
                return Json(new { success = false, message = "Đã tạo báo cáo của tháng hiện tại rồi, vui lòng không tạo lại" });
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result; // tạo 1 list customer role là khách hàng từ list user
            DebitDetail debitdetail = new DebitDetail();
            foreach (var item in customerlist)
            {
                if (item.new_last_debit != 0 || ((item.new_first_debit + item.new_incurred_debit) == 0 && item.new_first_debit !=0)) // cuối tháng có nợ , hoặc nợ cuối tháng = 0 vì nợ đầu - nợ phát sinh hết( nợ đầu phải khác 0)
                {
                    
                    debitdetail.Id = Guid.NewGuid().ToString();
                    debitdetail.ApplicationUserId = item.Id;
                    debitdetail.TimeRecord = timenow;
                    debitdetail.FirstDebit = item.new_first_debit;
                    debitdetail.IncurredDebit = item.new_incurred_debit;
                    debitdetail.LastDebit = item.new_last_debit;
                    _db.DebitDetails.Add(debitdetail);
                    _db.SaveChanges();
                    // lưu lại giá trị cũ để update
                    item.old_first_debit = item.new_first_debit;
                    item.old_incurred_debit = item.new_incurred_debit;
                    item.old_last_debit = item.new_last_debit;
                    _db.AppUsers.Update(item);
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
        public IActionResult Update_List_Debit()
        {
            var timenow = DateTime.Parse(DateTime.Now.ToString("MM-yyyy"));
            var result = IsAvailable_DebitDetail_Has_Time(timenow);
            if(result==true)
                return Json(new { success = false, message = "Chưa tạo báo cáo của tháng hiện tại, nên không thể cập nhật báo cáo được" });
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result; // tạo 1 list customer role là khách hàng từ list user
            DebitDetail debitdetail = new DebitDetail();
            // xóa các debit detail cũ của tháng đó đi
            Remove_debit_detail_in_month(timenow);
            foreach (var item in customerlist)
            {
                if (item.old_last_debit != 0 || ((item.old_first_debit + item.old_incurred_debit) == 0 && item.old_first_debit != 0)) // nếu đã tồn tại nợ cũ
                {
                    // cập nhật lại nợ phát sinh , nợ cuối của khách hàng
                    item.old_incurred_debit = item.old_incurred_debit + item.new_incurred_debit;
                    item.old_last_debit = item.old_first_debit + item.old_incurred_debit;
                    _db.AppUsers.Update(item);
                    _db.SaveChanges();
                    // add lại debit detail cho user với incurred mới
                    debitdetail.Id = Guid.NewGuid().ToString();
                    debitdetail.ApplicationUserId = item.Id;
                    debitdetail.TimeRecord = timenow;
                    debitdetail.FirstDebit = item.old_first_debit;
                    debitdetail.IncurredDebit = item.old_incurred_debit;
                    debitdetail.LastDebit = item.old_last_debit;
                    _db.DebitDetails.Add(debitdetail);
                    _db.SaveChanges();
                    // reset lại new debit sau khi update
                    item.new_first_debit = item.new_last_debit;
                    item.new_incurred_debit = 0;
                    item.new_last_debit = item.new_first_debit + item.new_incurred_debit;
                    _db.AppUsers.Update(item);
                    _db.SaveChanges(); 
                }
                else if (item.new_last_debit != 0 || ((item.new_first_debit + item.new_incurred_debit) == 0 && item.new_first_debit != 0)) // nếu nợ cũ không tồn tại và nợ mới thì có -> phải tạo mới
                {

                    debitdetail.Id = Guid.NewGuid().ToString();
                    debitdetail.ApplicationUserId = item.Id;
                    debitdetail.TimeRecord = timenow;
                    debitdetail.FirstDebit = item.new_first_debit;
                    debitdetail.IncurredDebit = item.new_incurred_debit;
                    debitdetail.LastDebit = item.new_last_debit;
                    _db.DebitDetails.Add(debitdetail);
                    _db.SaveChanges();
                    // lưu lại giá trị cũ để update
                    item.old_first_debit = item.new_first_debit;
                    item.old_incurred_debit = item.new_incurred_debit;
                    item.old_last_debit = item.new_last_debit;
                    _db.AppUsers.Update(item);
                    _db.SaveChanges();
                    item.new_first_debit = item.new_last_debit;
                    item.new_incurred_debit = 0;
                    item.new_last_debit = item.new_first_debit + item.new_incurred_debit;
                    _db.AppUsers.Update(item);
                    _db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "" });
        }
        public void Remove_debit_detail_in_month(DateTime time) // xóa các debit detail có tháng ghi là tháng đầu vào
        {
            var list_debit_detail_in_month = _db.DebitDetails.ToList();
            foreach(var item in list_debit_detail_in_month)
            {
                if(item.TimeRecord == time)
                {
                    _db.DebitDetails.Remove(item);
                    _db.SaveChanges();
                }
            }    
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
