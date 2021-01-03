using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Utility;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ReceiptController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _usermanager;
        public ReceiptController(ApplicationDbContext db, UserManager<AppUser> usermanager)
        {
            _db = db;
            _usermanager = usermanager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Getall()
        {
            var obj = _db.Receipts.Include(x=>x.ApplicationUser).ToList().Select(x => new
            {
                id = x.Id,
                customer = x.ApplicationUser.FullName,
                datecreate = x.DateCreate,
                proceed = x.Proceeds,
            });
            return Json(new { data = obj });
        }
        private void Add_ViewBag()
        {
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result;
            ViewBag.Customer = new SelectList(customerlist, "Id", "FullName");

            var localDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(' ', 'T');
            ViewBag.DateCreate = localDateTime;
        }
        public IActionResult Detail(string id)
        {
            var receipt = _db.Receipts.Include(x=>x.ApplicationUser).FirstOrDefault(x => x.Id == id);
            return View(receipt);
        }
        public IActionResult Create(Receipt receipt)
        {
            ViewBag.id = Guid.NewGuid().ToString();
            Add_ViewBag();
            if (receipt == null)
                receipt = new Receipt();
            return View(receipt);
        }
        public IActionResult GetInfoUser(string id)
        {
            var user = _db.AppUsers.FirstOrDefault(x => x.Id == id);
            if (user!= null)
            {
                return Json(new { success = true,address = user.Address, phonenumber = user.PhoneNumber , email = user.Email });
            }
            return Json(new { success = false, address = "", phonenumber = "", email = "" });

        }
        [HttpPost]
        public IActionResult Create(string id,string customer,DateTime date_create,float proceed)
        {
            Receipt receipt = new Receipt() { Id = id, ApplicationUserId = customer, DateCreate = date_create, Proceeds = proceed  };
            var cus = _db.AppUsers.FirstOrDefault(x => x.Id == customer);
            receipt.ApplicationUser = cus;
            try
            {
                if (customer == null)
                {
                    throw new Exception("Khách hàng không được để trống");
                }
                if (proceed == 0)
                {
                    throw new Exception("Số tiền thu không được để trống hoặc bằng 0");
                }
                check_rule_3(receipt);
                receipt.ApplicationUser = null;
                _db.Receipts.Add(receipt);
                _db.SaveChanges();
                cus.new_incurred_debit = cus.new_incurred_debit - receipt.Proceeds;
                cus.new_last_debit = cus.new_first_debit + cus.new_incurred_debit;
                _db.AppUsers.Update(cus);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_ViewBag();
                return View(receipt);
            }
        }
        
        private void check_rule_3(Receipt receipt)
        {
            var rule = _db.Rules.Find("QD3");
            if (rule.UseThisRule == true)
            {
                if (receipt.Proceeds > receipt.ApplicationUser.new_last_debit)
                    throw new Exception("Số tiền thu của khách không được vượt quá số tiền nợ hiện tại của khách hàng");
            }
        }
        //private bool Remove_Or_Update_Debit(Receipt receipt)
        //{
        //    var list_debit_detail = _db.DebitDetails.Where(x => x.ApplicationUserId == receipt.ApplicationUserId );
        //    DateTime nearest_date = DateTime.Parse("01-01-0001"); // khởi tạo ngày nợ nhỏ nhất có thể
        //    var date_create = DateTime.Parse(receipt.DateCreate.ToString("MM-yyyy"));
        //    DebitDetail debit = null;
        //    foreach (var item in list_debit_detail)
        //    {
        //        if ((item.TimeRecord - date_create).TotalDays <= 0 && (item.TimeRecord - nearest_date).TotalDays >= 0)
        //        {
        //            nearest_date = DateTime.Parse(item.TimeRecord.ToString("MM-yyyy")); // chuẩn hóa ngày gần nhất , để ngày = 1
        //            debit = item;
        //        }
        //    }
        //    if (debit == null)
        //    {
        //        return false; // không có nợ
        //    }
        //    else // có nợ
        //    {
        //        if(debit.LastDebit<=receipt.Proceeds) // tiền trả nợ ít hơn tiền nợ hoặc bằng tiền nợ
        //        {
        //            debit.IncurredDebit = debit.IncurredDebit - receipt.Proceeds;
        //            debit.LastDebit = debit.FirstDebit + debit.IncurredDebit;
        //            _db.DebitDetails.Update(debit);
        //            _db.SaveChanges();
        //        }
        //        else
        //        {

        //        }
        //    }


        //}
    }
}
