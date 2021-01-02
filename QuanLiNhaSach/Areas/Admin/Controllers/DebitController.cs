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
    }
     
}
