using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;

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
    }
}
