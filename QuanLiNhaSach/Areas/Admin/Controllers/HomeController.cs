using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            var obj1 = _db.Categories.Include(x => x.Books).Select(x => x.Name).ToArray();
            var obj = _db.Categories.Include(x => x.Books).Select(x => x.Books.Count()).ToArray();

            ViewBag.label = "[\""+String.Join("\",\"",obj1)+"\"]";
            ViewBag.data = "[" + String.Join(",", obj) + "]";



            return View();
        }

        
    }

}
