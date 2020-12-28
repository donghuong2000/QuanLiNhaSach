using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

            if(bill == null)
            {
                return NotFound();
            }
            var billDetails = _db.BillDetails.Include(x=>x.Book).Where(x=>x.BillId==id).Select(x=>x).ToList();

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

            bill.ApplicationUser = null;
            bill.Staff = null;
            var newBillDetail = bill.BillDetail.Select(x => new BillDetail { BillId = x.BillId, BookId = x.BookId, Count = x.Count  }).ToList();
            bill.BillDetail = newBillDetail;
            // add bill 
            _db.Bills.Add(bill);
            // them debit neu is debit = true
            if(bill.IsDebit==true)
            {
                if(IsAvailableTimeRecord(bill.ApplicationUserId,DateTime.Now)==false)
                {

                }
                var debit = new DebitDetail { Id = Guid.NewGuid().ToString(), ApplicationUserId = bill.ApplicationUserId, TimeRecord = DateTime.Now, };
                //_db.DebitDetails.Add()
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Create(string[] product,int[] qty,string customer,float total_amount,string check_debit) // check_debit : có check là on , uncheck là null
        {
            //get current userId
            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //get current userId
            var user = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == currentUserID);

            var cus = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == customer);


            bool isdebit = false;
            if (check_debit == "on")
                isdebit = true;
            var bill = new Bill() { ApplicationUserId = cus.Id, ApplicationUser = cus, Id = Guid.NewGuid().ToString(), Staff = user, StaffId = user.Id, DateCreate = DateTime.Now, TotalPrice = total_amount, IsDebit = isdebit };

            List<BillDetail> billDetails = new List<BillDetail>();
            for (int i = 0; i < product.Length; i++)
            {
                billDetails.Add(new BillDetail
                {
                    BillId = bill.Id,
                    BookId = product[i],
                    Count = qty[i],
                    Book = _db.Books.AsNoTracking().FirstOrDefault(x => x.Id == product[i])
                });
            }

            bill.BillDetail = billDetails;
            if (product.Contains(null))
            {
                Add_SelectList_For_ViewBag();
                ModelState.AddModelError("", "Không được để hàng trống");
                return View(bill);
            }
            if(customer==null)
            {
                Add_SelectList_For_ViewBag();
                ModelState.AddModelError("", "Vui lòng chọn user");
                return View(bill);
            }    
            
            try
            {
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
            
                var book = _db.Books.Find(id);
                if(book!=null)
                {
                    return Json(new { success = true, price = book.Price });
                }
                return Json(new { success = false, price = 0 });
            
        }

        public IActionResult Create()
        {
            Bill bill = new Bill();
            Add_SelectList_For_ViewBag();
            return View(bill);
        }
        private void Add_SelectList_For_ViewBag()
        {
            ViewBag.Books = new SelectList(_db.Books.ToList(), "Id", "Name");
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result;
            ViewBag.Customer = new SelectList(customerlist, "Id", "FullName");
        }
        private bool IsAvailableTimeRecord(string userid,DateTime timerecord)
        { 
            var list_debit_detail = _db.DebitDetails.ToList();
            foreach(var item in list_debit_detail)
            {
                if (item.TimeRecord == timerecord && item.ApplicationUserId == userid )
                    return true;
            }
            return false;
        }
        private void check_rule_2(Bill bill)
        {
            var rule = _db.Rules.Find("QD2");
            if (rule.UseThisRule == true)
            {
                foreach (var item in bill.BillDetail)
                {
                    if (_db.Books.Find(item.BookId).Quantity - item.Count - rule.Max < 0) // số lượng tồn sau khi bán của sách nếu có sử dụng rule 2 mà < số lượng tồn sau khi bán(quy định) thì sẽ văng ra lỗi
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số hai" + ",số lượng tồn kho của sách sau khi bán phải từ " + rule.Max + " quyển trở lên");
                    }
                }
                if (bill.ApplicationUser.Dept >= rule.Min) // nếu nợ của khách lớn hơn quy định cho phép
                    throw new Exception("Chỉ bán cho khách hàng có nợ không  quá" + rule.Min + "đồng");
            }
            else // nếu không sử dụng rule 2
            {
                // kiểm tra xem số lượng đặt mua có lớn hơn só lượng tồn của kho sách không
                foreach(var item in bill.BillDetail)
                {
                    if(item.Count > _db.Books.Find(item.BookId).Quantity)
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" có số lượng tồn kho không đủ để thực hiện tạo hóa đơn");
                    }
                }    
            }
        }
    }
}
