﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Books.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult SearchBook(string q)
        {
            var IncludeObj = _db.Books.Include(x => x.Category);

            if(q!=null && q!= "")
            {
                var obj = IncludeObj.Where(
                    x => x.Name.ToLower().Contains(q.ToLower().Trim()) || 
                    x.Decription.ToLower().Contains(q.ToLower().Trim()) || 
                    x.Author.ToLower().Contains(q.ToLower().Trim()) || 
                    x.Category.Name.ToLower().Contains(q.ToLower().Trim())
                    );
                return Json(new { items = obj.ToList() });
            }
            return Json(new { items = IncludeObj.ToList() });

        }
    }
}
