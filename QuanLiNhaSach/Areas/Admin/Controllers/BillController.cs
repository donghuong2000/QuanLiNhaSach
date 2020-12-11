using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public BillController(ApplicationDbContext db)
        {
            _db = db;
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
            var newBillDetail = bill.BillDetail.Select(x => new BillDetail { BillId = x.BillId, BookId = x.BookId, Count = x.Count }).ToList();

            bill.BillDetail = newBillDetail;
           
            _db.Bills.Add(bill);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Create(string[] product,int[] qty,string customer,float total_amount)
        {

            if(product.Contains(null))
            {
                ModelState.AddModelError("", "Không được để hàng trống");
                return View();
            }
            if(customer==null)
            {
                ModelState.AddModelError("", "Vui lòng chọn user");
                return View();
            }    
            //get current userId
            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //get current userId
            var user = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == currentUserID);

            var cus = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == customer);



            var bill = new Bill() { ApplicationUserId = cus.Id, ApplicationUser = cus, Id = Guid.NewGuid().ToString(), Staff = user, StaffId = user.Id, DateCreate = DateTime.Now, TotalPrice = total_amount };

            List<BillDetail> billDetails = new List<BillDetail>();
            for (int i = 0; i < product.Length; i++)
            {
                billDetails.Add(new BillDetail 
                {   BillId = bill.Id,
                    BookId = product[i], 
                    Count = qty[i], 
                    Book = _db.Books.AsNoTracking().FirstOrDefault(x=>x.Id == product[i]) 
                });
            }

            bill.BillDetail = billDetails;


            HttpContext.Session.SetObject("bill", bill);
            return RedirectToAction("BillDemo");
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

            ViewBag.Books = new SelectList(_db.Books.ToList(), "Id", "Name");
            ViewBag.Customer= new SelectList(_db.AppUsers.ToList(), "Id", "FullName");
            return View();
        }
    }
}
