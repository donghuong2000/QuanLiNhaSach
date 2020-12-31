using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLiNhaSach.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Route("Books/{id}")]
        public IActionResult Index(string id)
        {
            try
            {
                return View(_db.Books.Include(x=>x.Category).FirstOrDefault(x=>x.Id == id));
            }
            catch (Exception)
            {

                return NotFound();
            }
            
        }
    }
}
