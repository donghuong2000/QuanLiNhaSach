using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Utility;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
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
                var newTicketDetail = ticket.BookEntryTicketDetail.Select(x => new BookEntryTicketDetail { BookEntryTicketId = x.BookEntryTicketId, BookId = x.BookId, Count = x.Count }).ToList();

                ticket.BookEntryTicketDetail = newTicketDetail;
                _db.BookEntryTickets.Add(ticket);
                foreach (var item in ticket.BookEntryTicketDetail) // add số lượng từ đơn nhập sách cho các sách 
                {
                    // tìm cuốn sách đó trong danh sách sách
                    var b = _db.Books.Find(item.BookId);
                    // tăng số lượng cuốn sách đó đúng bằng số lượng trong đơn nhập sách
                    b.new_incurred_exist += item.Count;
                    b.Quantity = b.new_first_exist + b.new_incurred_exist;
                    _db.Books.Update(b); // cập nhật lại thay đổi
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return RedirectToAction("Create", new { ticket = ticket });
            }


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
                    var obj = new BookEntryTicketDetail
                    {
                        BookEntryTicketId = ticket.Id,
                        BookId = listticketdetail_update[i].BookId,
                        Count = listticketdetail_update[i].Count,
                        Book = _db.Books.AsNoTracking().Include(x => x.Category).FirstOrDefault(x => x.Id == product[i]),
                    };
                    obj.Book.Category.Books = null;
                    ticketDetails.Add(obj);
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
                BookEntryTicketDetail ticketdetail = new BookEntryTicketDetail() { BookId = product[i], Count = qty[i] };
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
                return Json(new { success = true, category = book.Category.Name, author = book.Author });
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
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số một" + ",chỉ nhập sách khi số lượng tồn kho còn dưới " + rule.Max + "quyển.");
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
//        //// xử lý tồn sách
//        //private bool BookExist (BookEntryTicket ticket)
//        //{


//        //    var list_BookInTicket = _db.BookEntryTicketDetails.Include(x => x.Book).ToList();
//        //    foreach ( var bookin in list_BookInTicket)
//        //    {
//        //        BookExistDetail existdetail = null;
//        //        int qtyEntry = bookin.Count;
//        //        DateTime nearest_record = bookin.Book.DatePublish;
//        //        var list_exist_record = _db.BookExistDetails.Where(x => x.BookExistHeaderId == bookin.BookId).ToList();
//        //        bool check_break = false;
//        //        foreach( var item in list_exist_record)
//        //        {
//        //            var time = item.TimeRecord.ToString("MM-yyyy");
//        //            if (time == ticket.DateEntry.ToString("MM-yyyy")) // nếu có data tồn kho có tháng trùng với tháng ngày nhập sách thì chỉ cần update tháng đó
//        //            {
//        //                item.IncurredExist += qtyEntry; 
//        //                item.LastExist = item.FirstExist + item.IncurredExist; 
//        //                _db.BookExistDetails.Update(item);
//        //                _db.SaveChanges();
//        //                check_break = true;
//        //                break;
//        //            }
//        //            else if ((item.TimeRecord - ticket.DateEntry).TotalDays < 0 && (item.TimeRecord - nearest_record).TotalDays >= 0) 
//        //            {
//        //                nearest_record = DateTime.Parse(item.TimeRecord.ToString("MM-yyyy")); 
//        //                existdetail = item;
//        //            }// giúp gán existdetail thành data gần nhất so với tháng lập phiếu
//        //        }
//        //        // chạy xong foreach này: 1 là chỉ update tồn sách 1 tháng, 2 là gán existdetail tới data tồn gàn nhất
//        //        if (!check_break)
//        //        {

//        //            var list_month = new List<DateTime>(); // list các tháng 
//        //            var datestart = nearest_record.AddMonths(1); // thêm 1 tháng vào ngày gần nhất 
//        //            var dateend = DateTime.Parse(ticket.DateEntry.ToString("MM-yyyy"));
//        //            for (var dt = datestart; dt < dateend; dt = dt.AddMonths(1)) // tạo ra 1 list tháng sao cho > hơn nearestdate và nhỏ hơn ngày tạo bill
//        //            {
//        //                list_month.Add(DateTime.Parse(dt.ToString("MM-yyyy")));
//        //            }
//        //            BookExistDetail newexist = new BookExistDetail();
//        //            newexist.BookExistHeaderId = bookin.Book.Id;
//        //            newexist.TimeRecord = DateTime.Parse(ticket.DateEntry.ToString("MM-yyyy"));
//        //            if (existdetail != null) //nếu đã từng nhập sách, list_exist_record không null
//        //            {
//        //                // tạo tồn sách của tháng hiện tại
//        //                newexist.Id = Guid.NewGuid().ToString();
//        //                newexist.FirstExist = existdetail.LastExist;
//        //                newexist.IncurredExist = qtyEntry;
//        //                newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//        //                _db.BookExistDetails.Add(newexist);
//        //                _db.SaveChanges();
//        //                foreach (var mth in list_month)  // tạo data tồn cho các tháng chưa có từ tháng có data tồn gần nhất tới tháng có tương tác
//        //                {
//        //                    newexist.Id = Guid.NewGuid().ToString();
//        //                    newexist.TimeRecord = mth;  // lưu time record chỉ có tháng năm , không cần ngày
//        //                    newexist.FirstExist = existdetail.LastExist;
//        //                    newexist.IncurredExist = 0;
//        //                    newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//        //                    _db.BookExistDetails.Add(newexist);
//        //                    _db.SaveChanges();
//        //                }
//        //            } 
//        //            else // nếu đã tới else này thì đây là lần nhập đầu tiên
//        //            {
//        //                newexist.Id = Guid.NewGuid().ToString();
//        //                newexist.FirstExist = 0;
//        //                newexist.IncurredExist = qtyEntry;
//        //                newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//        //                _db.BookExistDetails.Add(newexist);
//        //                _db.SaveChanges();
//        //            }
//        //        }
//        //    }
//        //    _db.SaveChanges();
//        //    return true;

//            var list_BookInTicket = _db.BookEntryTicketDetails.Include(x => x.Book).ToList();
//            foreach ( var bookin in list_BookInTicket)
//            {
//                BookExistDetail existdetail = null;
//                int qtyEntry = bookin.Count;
//                DateTime nearest_record = bookin.Book.DatePublish;
//                var list_exist_record = _db.BookExistDetails.Where(x => x.BookId == bookin.BookId).ToList();
//                bool check_break = false;
//                foreach( var item in list_exist_record)
//                {
//                    var time = item.TimeRecord.ToString("MM-yyyy");
//                    if (time == ticket.DateEntry.ToString("MM-yyyy")) // nếu có data tồn kho có tháng trùng với tháng ngày nhập sách thì chỉ cần update tháng đó
//                    {
//                        item.IncurredExist += qtyEntry; 
//                        item.LastExist = item.FirstExist + item.IncurredExist; 
//                        _db.BookExistDetails.Update(item);
//                        _db.SaveChanges();
//                        check_break = true;
//                        break;
//                    }
//                    else if ((item.TimeRecord - ticket.DateEntry).TotalDays < 0 && (item.TimeRecord - nearest_record).TotalDays >= 0) 
//                    {
//                        nearest_record = DateTime.Parse(item.TimeRecord.ToString("MM-yyyy")); 
//                        existdetail = item;
//                    }// giúp gán existdetail thành data gần nhất so với tháng lập phiếu
//                }
//                // chạy xong foreach này: 1 là chỉ update tồn sách 1 tháng, 2 là gán existdetail tới data tồn gàn nhất
//                if (!check_break)
//                {

//                    var list_month = new List<DateTime>(); // list các tháng 
//                    var datestart = nearest_record.AddMonths(1); // thêm 1 tháng vào ngày gần nhất 
//                    var dateend = DateTime.Parse(ticket.DateEntry.ToString("MM-yyyy"));
//                    for (var dt = datestart; dt < dateend; dt = dt.AddMonths(1)) // tạo ra 1 list tháng sao cho > hơn nearestdate và nhỏ hơn ngày tạo bill
//                    {
//                        list_month.Add(DateTime.Parse(dt.ToString("MM-yyyy")));
//                    }
//                    BookExistDetail newexist = new BookExistDetail();
//                    newexist.BookId = bookin.Book.Id;
//                    newexist.TimeRecord = DateTime.Parse(ticket.DateEntry.ToString("MM-yyyy"));
//                    if (existdetail != null) //nếu đã từng nhập sách, list_exist_record không null
//                    {
//                        // tạo tồn sách của tháng hiện tại
//                        newexist.Id = Guid.NewGuid().ToString();
//                        newexist.FirstExist = existdetail.LastExist;
//                        newexist.IncurredExist = qtyEntry;
//                        newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//                        _db.BookExistDetails.Add(newexist);
//                        _db.SaveChanges();
//                        foreach (var mth in list_month)  // tạo data tồn cho các tháng chưa có từ tháng có data tồn gần nhất tới tháng có tương tác
//                        {
//                            newexist.Id = Guid.NewGuid().ToString();
//                            newexist.TimeRecord = mth;  // lưu time record chỉ có tháng năm , không cần ngày
//                            newexist.FirstExist = existdetail.LastExist;
//                            newexist.IncurredExist = 0;
//                            newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//                            _db.BookExistDetails.Add(newexist);
//                            _db.SaveChanges();
//                        }
//                    } 
//                    else // nếu đã tới else này thì đây là lần nhập đầu tiên
//                    {
//                        newexist.Id = Guid.NewGuid().ToString();
//                        newexist.FirstExist = 0;
//                        newexist.IncurredExist = qtyEntry;
//                        newexist.LastExist = newexist.FirstExist + newexist.IncurredExist;
//                        _db.BookExistDetails.Add(newexist);
//                        _db.SaveChanges();
//                    }
//                }
//            }
//            _db.SaveChanges();
//            return true;


//        //}
//    }
//}
