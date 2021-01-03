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
    public class BookExistController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BookExistController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll(string mth)
        {
            var obj = _db.BookExistDetails
                .Select(x => new
                {
                    id = x.Id,
                    book = x.Book.Name,
                    date = x.TimeRecord.ToString("MM-yyyy"),
                    firstexist = x.FirstExist,
                    incurredexist = x.IncurredExist,
                    lastexist = x.LastExist,
                }).ToList();
          ;
              
            if (mth != null)
            {
                var time = DateTime.Parse(mth).ToString("MM-yyyy");
                var exist = obj.Where(x => x.date == time).ToList();
                return Json(new { data = exist });
            }
            return Json(new { data = obj });
        }
    }
     
}
