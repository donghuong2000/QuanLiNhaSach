using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class RuleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RuleController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var rulelist = _db.Rules.ToList();
            return View(rulelist);

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(string id,bool utr,int min,int max)
        {
            // utr = use this rule
            // nin = nhập ít nhất
            // tin = tồn ít nhất
            try
            {
                var rule = _db.Rules.Find(id);
                rule.UseThisRule = utr;
                rule.Min = min;
                rule.Max = max;

                _db.Update(rule);
                _db.SaveChanges();
                return Json(new { success = true, message = "đã cập nhập thành công quy định này" });

            }
            catch (Exception e)
            {

                return Json(new { success = false, message =e.Message });
            }
        }

        

    }
}
