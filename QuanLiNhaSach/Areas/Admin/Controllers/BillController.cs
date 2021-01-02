
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _usermanager;

        public BillController(ApplicationDbContext db, UserManager<AppUser> usermanager)
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
            var obj = _db.Bills
                .Include(x => x.ApplicationUser)
                .Include(x => x.Staff)
                .Select(x => new
                {
                    id = x.Id,
                    staff = x.Staff.FullName,
                    customer = x.ApplicationUser.FullName,
                    date = x.DateCreate.ToShortDateString(),
                    total = x.TotalPrice
                });
            return Json(new { data = obj });
        }


        public IActionResult Detail(string id)
        {
            var bill = _db.Bills
                .Include(x => x.ApplicationUser)
                .Include(x => x.Staff)
                .FirstOrDefault(x => x.Id == id);

            if (bill == null)
            {
                return NotFound();
            }
            var billDetails = _db.BillDetails.Include(x => x.Book).Where(x => x.BillId == id).Select(x => x).ToList();

            bill.BillDetail = billDetails;
            return View(bill);

        }
        public IActionResult BillDemo()
        {
            var bill = HttpContext.Session.GetObject<Bill>("bill");
            return View(bill);

        }
        public IActionResult BillDemoCancel()
        {
            HttpContext.Session.Remove("bill");
            return RedirectToAction("Create");

        }
        public IActionResult BillDemoConfirm()
        {
            var bill = HttpContext.Session.GetObject<Bill>("bill");
            HttpContext.Session.Remove("bill");
            try
            {
                check_rule_2(bill);

                bill.ApplicationUser = null;
                bill.Staff = null;
                var newBillDetail = bill.BillDetail.Select(x => new BillDetail { BillId = x.BillId, BookId = x.BookId, Count = x.Count }).ToList();
                bill.BillDetail = newBillDetail;
                // add bill 
                _db.Bills.Add(bill);
                foreach (var item in bill.BillDetail)
                {
                    var b = _db.Books.Find(item.BookId);
                    b.Quantity -= item.Count;
                }
                if (bill.IsDebit == true) // người dùng chọn nợ thay vì trả tiền mặt
                {
                    var result = Creat_Or_Update_Debit(bill); // tạo nợ mới - nếu tháng đó chưa nợ , hoặc update nợ cho incurred của tháng nợ đó nếu đã có 
                    if (result == true)
                    {
                        var user = _db.AppUsers.FirstOrDefault(x => x.Id == bill.ApplicationUserId);
                        user.Dept += bill.TotalPrice;
                        _db.AppUsers.Update(user);
                        _db.SaveChanges(); // thêm nợ cho khách hàng
                    }


                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();

                return RedirectToAction("Create",new { bill = bill });
            }
            bill.ApplicationUser = null;
            bill.Staff = null;
            var newBillDetail = bill.BillDetail.Select(x => new BillDetail { BillId = x.BillId, BookId = x.BookId, Count = x.Count  }).ToList();
            bill.BillDetail = newBillDetail;
            // add bill 
            _db.Bills.Add(bill);
            if (bill.IsDebit == true) // người dùng chọn nợ thay vì trả tiền mặt
            {
                var user = _db.AppUsers.FirstOrDefault(x => x.Id == bill.ApplicationUserId);
                user.new_incurred_debit += bill.TotalPrice;
                user.new_last_debit = user.new_first_debit + user.new_incurred_debit; 
                _db.AppUsers.Update(user);
                _db.SaveChanges(); // thêm nợ cho khách hàng

            }
        }
        [HttpPost]
        public IActionResult Create(string[] product, int[] qty, string customer, float total_amount, string check_debit, DateTime time_create) // check_debit : có check là on , uncheck là null
        {
            Bill bill = new Bill();
            try
            {
                List<BillDetail> billDetails = new List<BillDetail>();
                var listbilldetail_update = Get_List_Bill_Detail_Standardized_Quantity(product, qty);
                for (int i = 0; i < listbilldetail_update.Count; i++)
                {
                    var b = new BillDetail
                    {
                        BillId = bill.Id,
                        BookId = listbilldetail_update[i].BookId,
                        Count = listbilldetail_update[i].Count,
                        Book = _db.Books.AsNoTracking().Include(x => x.Category).FirstOrDefault(x => x.Id == listbilldetail_update[i].BookId)
                    };
                    b.Book.Category.Books = null;
                    billDetails.Add(b);
                }

                bill.BillDetail = billDetails;
                var cus = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == customer);
                bill.ApplicationUser = cus;
                bool isdebit = false;
                if (check_debit == "on")
                    isdebit = true;
                bill.IsDebit = isdebit;


                if (product.Contains(null))
                {
                    throw new Exception("Không được để hàng trống");
                }
                if (qty.Contains(0))
                {
                    throw new Exception("Không được để số lượng của sách = 0");
                }
                if (customer == null)
                {
                    throw new Exception("Vui lòng chọn user");
                }
                //get current userId
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                //get current userId
                var user = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == currentUserID);

                //bill = new Bill() { ApplicationUserId = cus.Id, ApplicationUser = cus, Id = Guid.NewGuid().ToString(), Staff = user, StaffId = user.Id, DateCreate = DateTime.Now, TotalPrice = total_amount, IsDebit = isdebit };
                bill.ApplicationUserId = cus.Id;
                bill.Id = Guid.NewGuid().ToString();
                bill.Staff = user;
                bill.StaffId = user.Id;
                bill.DateCreate = time_create;
                bill.TotalPrice = total_amount;


                check_rule_2(bill);
                HttpContext.Session.SetObject("bill", bill);
                return RedirectToAction("BillDemo");
            }

            catch (Exception e)
            {



                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return View(bill);

            }


        }


        public IActionResult GetBookPrice(string id)
        {


            var book = _db.Books.Include(x => x.Category).FirstOrDefault(x => x.Id == id);
            if (book != null)
            {
                return Json(new { success = true, price = book.Price, category = book.Category.Name });
            }
            return Json(new { success = false, price = 0, category = "" });

        }

        public IActionResult Create(Bill bill)
        {
            if (bill == null)
                bill = new Bill();
            Add_SelectList_For_ViewBag();
            return View(bill);
        }
        private void Add_SelectList_For_ViewBag()
        {
            ViewBag.Books = new SelectList(_db.Books.ToList(), "Id", "Name");
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result;
            ViewBag.Customer = new SelectList(customerlist, "Id", "FullName");
            var localDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(' ', 'T');
            ViewBag.DateCreate = localDateTime;
        }
        //private bool Creat_Or_Update_Debit(Bill bill)
        //{
        //    DateTime nearest_date = DateTime.Parse("01-01-0001"); // khởi tạo ngày nợ nhỏ nhất có thể
        //    var list_debit_detail = _db.DebitDetails.Where(x => x.ApplicationUserId == bill.ApplicationUserId).ToList(); // tim cac ma no cua thang no nay
        //    //DebitDetail debit = null; // khoi tao 1 debit rong
        //    foreach (var item in list_debit_detail)
        //    {
        //        //var time = item.TimeRecord.ToString("MM-yyyy");


        //        // tồn tại tháng nợ trùng hóa đơn
        //        if (item.TimeRecord.ToString("MM-yyyy") == bill.DateCreate.ToString("MM-yyyy")) // tìm thằng mã nợ có tháng trùng với tháng của hóa đơn, và người mua hóa đơn là người nợ của mã đó
        //        {
        //            item.IncurredDebit += bill.TotalPrice; // cập nhật lại số nợ gia tăng của tháng đó
        //            item.LastDebit = item.FirstDebit + item.IncurredDebit; // cập nhật lại nợ cuối của tháng đó
        //            _db.DebitDetails.Update(item);
        //            _db.SaveChanges();
        //        }
        //        //else if ((item.TimeRecord - bill.DateCreate).TotalDays < 0 && (item.TimeRecord - nearest_date).TotalDays >= 0)
        //        //{
        //        //    nearest_date = DateTime.Parse(item.TimeRecord.ToString("MM-yyyy")); // chuẩn hóa ngày gần nhất , để ngày = 1
        //        //    debit = item;
        //        //}
        //    }
        //    return true;
        //    //var list_dates_debit = new List<DateTime>(); // list ngay de tao list debit 
        //    //var datestart = nearest_date.AddMonths(1); // thêm 1 tháng vào ngày gần nhất 
        //    //var dateend = DateTime.Parse(bill.DateCreate.ToString("MM-yyyy"));
        //    //for (var dt = datestart; dt < dateend; dt = dt.AddMonths(1)) // tạo ra 1 list tháng sao cho > hơn nearestdate và nhỏ hơn ngày tạo bill
        //    //{
        //    //    list_dates_debit.Add(DateTime.Parse(dt.ToString("MM-yyyy")));
        //    //}
        //    //DebitDetail newdebit = new DebitDetail();
        //    //newdebit.ApplicationUserId = bill.ApplicationUserId;
        //    //newdebit.TimeRecord = DateTime.Parse(bill.DateCreate.ToString("MM-yyyy"));
        //    ////newdebit.TimeRecord = DateTime.Parse(bill.DateCreate.ToString("MM-yyyy")); // lưu time record chỉ có tháng năm , không cần ngày
        //    //if (debit != null) // nếu tìm được ngày lớn nhất có thể nhưng không trùng với ngày hiện tại, đã từng nợ
        //    //{
        //    //    // tạo debit của tháng hiện tại( tháng hóa đơn)
        //    //    newdebit.Id = Guid.NewGuid().ToString();
        //    //    newdebit.FirstDebit = debit.LastDebit;
        //    //    newdebit.IncurredDebit = bill.TotalPrice;
        //    //    newdebit.LastDebit = newdebit.FirstDebit + newdebit.IncurredDebit;
        //    //    _db.DebitDetails.Add(newdebit);
        //    //    _db.SaveChanges();
        //    //    foreach (var item in list_dates_debit)  // tạo debit của tháng sau tháng gần nhất và trước tháng hiện tại( tháng tạo hóa đơn)
        //    //    {
        //    //        newdebit.Id = Guid.NewGuid().ToString();
        //    //        newdebit.TimeRecord = item;  // lưu time record chỉ có tháng năm , không cần ngày
        //    //        newdebit.FirstDebit = debit.LastDebit;
        //    //        newdebit.IncurredDebit = 0;
        //    //        newdebit.LastDebit = newdebit.FirstDebit + newdebit.IncurredDebit;
        //    //        _db.DebitDetails.Add(newdebit);
        //    //        _db.SaveChanges();
        //    //    }

        //    //}
        //    //else
        //    //{
        //    //    newdebit.Id = Guid.NewGuid().ToString();
        //    //    newdebit.FirstDebit = 0;
        //    //    newdebit.IncurredDebit = bill.TotalPrice;
        //    //    newdebit.LastDebit = newdebit.FirstDebit + newdebit.IncurredDebit;
        //    //    _db.DebitDetails.Add(newdebit);
        //    //    _db.SaveChanges();
        //    //}
        //    //return true;
        //}
        

        //public float Find_First_Debit_Of_User(Bill bill)
        //{
        //    var list_debit_detail = _db.DebitDetails.ToList();

        //    string debit_detail_id = "";
        //    foreach (var item in list_debit_detail)
        //    {
        //        // tim id debit detail co ngay gan voi ngay hien tai nhat
        //        if (item.ApplicationUserId == bill.ApplicationUserId && ((item.TimeRecord - nearest_date).TotalDays >= 0))
        //        {
        //            debit_detail_id = item.Id;
        //        }
        //    }
        //    var first_debit = _db.DebitDetails.FirstOrDefault(x => x.Id == debit_detail_id).LastDebit;
        //    return first_debit;
        //}
        private void check_rule_2(Bill bill)
        {
            var rule = _db.Rules.Find("QD2");
            if (rule.UseThisRule == true)
            {
                foreach (var item in bill.BillDetail)
                {
                    if (_db.Books.Find(item.BookId).Quantity - item.Count - rule.Max < 0) // số lượng tồn sau khi bán của sách nếu có sử dụng rule 2 mà < số lượng tồn sau khi bán(quy định) thì sẽ văng ra lỗi
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số hai" + ",số lượng tồn kho của sách sau khi bán phải từ " + rule.Max + " quyển trở lên (Chỉ có thể mua tối đa " + (_db.Books.Find(item.BookId).Quantity - rule.Max) + " quyển)");
                    }
                }
                if (bill.ApplicationUser.new_last_debit > rule.Min) // nếu nợ của khách lớn hơn quy định cho phép
                    throw new Exception("Chỉ bán cho khách hàng có nợ không  quá" + rule.Min + "đồng");

                else if (bill.IsDebit == true && (bill.ApplicationUser.new_last_debit + bill.TotalPrice > rule.Min)) // nếu nợ của khách nhỏ hơn quy định cho phép nhưng khách lại muốn tiếp tục nợ mà tiền sách nợ + với nợ cũ lớn hơn quy định cho phép thì cũng không bán
                    throw new Exception("Khách hàng hiện đang nợ " + bill.ApplicationUser.new_last_debit + " đồng. Nếu tiếp tục mua nợ thì nợ của khách hàng sẽ là " + (bill.ApplicationUser.new_last_debit + bill.TotalPrice) + " đồng.Vi phạm quy định 2. Để tiếp tục giao dịch, Vui lòng thanh toán nợ cho thủ thư,hoặc chọn phương thức mua không nợ");
             }

            else // nếu không sử dụng rule 2
            {
                // kiểm tra xem số lượng đặt mua có lớn hơn só lượng tồn của kho sách không
                foreach (var item in bill.BillDetail)
                {
                    if (item.Count > _db.Books.Find(item.BookId).Quantity)
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" có số lượng tồn kho không đủ để thực hiện tạo hóa đơn, ( Chỉ có thể mua tối đa " + _db.Books.Find(item.BookId).Quantity + " quyển )");
                    }
                }
            }
        }
        private List<BillDetail> Get_List_Bill_Detail_Standardized_Quantity(string[] product, int[] quantity)
        {
            List<BillDetail> listbilldetail = new List<BillDetail>();
            for (int i = 0; i < product.Length; i++)
            {
                if (product[i] != null)
                {
                    BillDetail billdetail = new BillDetail() { BookId = product[i], Count = quantity[i] };
                    listbilldetail.Add(billdetail);
                }


            } // tao ra 1 list product tu 2 mang product va quantity
            var newlistbilldetail = listbilldetail.GroupBy(x => x.BookId)
                .Select(x => new
                {
                    bookid = x.Key,
                    count = x.Sum(y => y.Count)
                }).ToList();

            var listbilldetail_update = newlistbilldetail.Select(x => new BillDetail { BookId = x.bookid, Count = x.count }).ToList();
            return listbilldetail_update;

        }
        //private void standard_list_debit_detail_after_add_or_update() // chuẩn hóa danh sách debit detail khi add thêm nợ cho 1 debit detail, hoặc tạo mới 1 debit detail
        //{
        //    var list_debit_detail = _db.DebitDetails.ToList();
        //    foreach()
        //}

    }
}
