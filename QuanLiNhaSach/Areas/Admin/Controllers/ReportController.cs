using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult BookInCategory()
        {
           var obj= _db.Categories.Include(x => x.Books).Select(x => new
            {

                name = x.Name,
                count = x.Books.Count
            }).ToList();

            
            return Json(obj);
        }
    }
}
