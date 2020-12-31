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
    public class BookEntryTicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BookEntryTicketController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Getall()
        {
            var obj = _db.BookEntryTickets
                .Select(x => new
                {
                    id = x.Id,
                    date = x.DateEntry.ToShortDateString(),
                });
            return Json(new { data = obj });
        }
        public IActionResult Detail(string id)
        {
            var ticket = _db.BookEntryTickets
                .FirstOrDefault(x => x.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            var ticketDetails = _db.BookEntryTicketDetails
                .Include(x => x.Book)
                .Include(x => x.Book.Category)
                .Where(x => x.BookEntryTicketId == id)
                .Select(x => x).ToList();

            ticket.BookEntryTicketDetail = ticketDetails;
            return View(ticket);
        }
        public IActionResult TicketDemo()
        {
            var ticket = HttpContext.Session.GetObject<BookEntryTicket>("ticket");
            return View(ticket);

        }
        public IActionResult TicketDemoCancel()
        {
            HttpContext.Session.Remove("ticket");
            return RedirectToAction("Create");

        }
        public IActionResult TicketDemoConfirm()
        {
            var ticket = HttpContext.Session.GetObject<BookEntryTicket>("ticket");
            HttpContext.Session.Remove("ticket");

            try
            {
                check_rule_1(ticket);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return RedirectToAction("Create", new { ticket = ticket });
            }

            var newTicketDetail = ticket.BookEntryTicketDetail.Select(x => new BookEntryTicketDetail { BookEntryTicketId = x.BookEntryTicketId, BookId = x.BookId, Count = x.Count }).ToList();

            ticket.BookEntryTicketDetail = newTicketDetail;

            _db.BookEntryTickets.Add(ticket);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void Add_SelectList_For_ViewBag()
        {
            ViewBag.Books = new SelectList(_db.Books.ToList(), "Id", "Name");
        }

        [HttpPost]
        public IActionResult Create(string[] product, int[] qty, string[] category)
        {
            BookEntryTicket ticket = new BookEntryTicket();
            if (product.Contains(null) || qty.Contains(0))
            {
                ModelState.AddModelError("", "Vui lòng chọn sách và nhập số lượng đầy đủ");
                Add_SelectList_For_ViewBag();
                return View(ticket);
            }
            try
            {
                List<BookEntryTicketDetail> ticketDetails = new List<BookEntryTicketDetail>();
                var listticketdetail_update = Get_List_Ticket_Detail_Standardized_Quantity(product, qty);
                for (int i = 0; i < listticketdetail_update.Count; i++)
                {
                    ticketDetails.Add(new BookEntryTicketDetail
                    {
                        BookEntryTicketId = ticket.Id,
                        BookId = listticketdetail_update[i].BookId,
                        Count = listticketdetail_update[i].Count,
                        Book = _db.Books.Include(x => x.Category).AsNoTracking().FirstOrDefault(x => x.Id == product[i]),
                    }); ;
                }
                ticket.BookEntryTicketDetail = ticketDetails;
                ticket.Id = Guid.NewGuid().ToString();
                ticket.DateEntry = DateTime.Now;

                check_rule_1(ticket);
                HttpContext.Session.SetObject("ticket", ticket);
                return RedirectToAction("TicketDemo");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return View(ticket);

            }
        }
        private List<BookEntryTicketDetail> Get_List_Ticket_Detail_Standardized_Quantity(string[] product, int[] qty)
        {
            List<BookEntryTicketDetail> listticketdetail = new List<BookEntryTicketDetail>();
            for (int i = 0; i < product.Length; i++)
            {
                BookEntryTicketDetail ticketdetail = new BookEntryTicketDetail() { BookId = product[i], Count = qty[i]};
                listticketdetail.Add(ticketdetail);
            } // tao ra 1 list product tu 2 mang product va quantity
            var newlistticketdetail = listticketdetail.GroupBy(x => x.BookId)
                .Select(x => new
                {
                    bookid = x.Key,
                    count = x.Sum(y => y.Count)                    
                }).ToList();

            var listticketdetail_update = newlistticketdetail.Select(x => new BookEntryTicketDetail { BookId = x.bookid, Count = x.count }).ToList();
            return listticketdetail_update;
        }

        public IActionResult Create(BookEntryTicket ticket)
        {
            if (ticket == null)
                ticket = new BookEntryTicket();
            Add_SelectList_For_ViewBag();
            return View(ticket);
        }
        public IActionResult GetBookInfo(string id)
        {

            var book = _db.Books
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == id);
            if (book != null)
            {
                return Json(new { success = true, category = book.Category.Name , author = book.Author});
            }
            return Json(new { success = false, category = "", author = "" });

        }
        private void check_rule_1(BookEntryTicket ticket)
        {
            var rule = _db.Rules.Find("QD1");
            if (rule.UseThisRule == true)
            {
                foreach (var item in ticket.BookEntryTicketDetail)
                {

                    if (_db.Books.Find(item.BookId).Quantity >= rule.Max) // chỉ nhập khi đạt số lượng tối thiểu
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số một" + ",chỉ nhập sách khi số lượng tồn kho còn dưới " + rule.Max  + "quyển.");
                    }
                    if (item.Count < rule.Min) // chỉ nhập khi đạt số lượng tối thiểu
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số một" + ", số lượng sách nhập vào ít nhất là" + rule.Min + "quyển.");
                    }

                }
            }

        }
    }
}
