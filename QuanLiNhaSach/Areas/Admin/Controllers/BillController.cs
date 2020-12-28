using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Data;
using QuanLiNhaSach.Models;
using QuanLiNhaSach.Utility;

namespace QuanLiNhaSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _usermanager;

        public BillController(ApplicationDbContext db, UserManager<AppUser> usermanager)
        {
            _db = db;
            _usermanager = usermanager;
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
        public IActionResult BillDemo()
        {
            var bill = HttpContext.Session.GetObject<Bill>("bill");
            return View(bill);

        }
        public IActionResult BillDemoCancel()
        {
            HttpContext.Session.Remove("bill");
            return RedirectToAction("Create");

        }
        public IActionResult BillDemoConfirm()
        {
            var bill = HttpContext.Session.GetObject<Bill>("bill");
            HttpContext.Session.Remove("bill");
            try
            {
                check_rule_2(bill);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return RedirectToAction("Create",new { bill = bill });
            }
            bill.ApplicationUser = null;
            bill.Staff = null;
            var newBillDetail = bill.BillDetail.Select(x => new BillDetail { BillId = x.BillId, BookId = x.BookId, Count = x.Count  }).ToList();
            bill.BillDetail = newBillDetail;
            // add bill 
            _db.Bills.Add(bill);
            // them debit neu is debit = true
            if(bill.IsDebit==true) // người dùng chọn nợ thay vì trả tiền mặt
            {
                if(IsAvailableTimeRecord(bill.ApplicationUserId,bill.DateCreate)==false) // nếu ngày lập hóa đơn không tồn tại trong debit detail time record, thì có 
                    // trường hợp xảy ra : 1 là không có tháng lập hóa đơn nào trước ngày hiện tại( thì first debit sẽ = 0 ; inccured debit = tiền của hóa đơn ; last debit = first debit + inccured debit
                    // trường hợp thứ 2 là có tồn tại tháng nợ cũ trước ngày hiện tại thì first debit = last debit của tháng cũ, inccured debit = tiền của hóa đơn ; last debit = first debit + incurred debit
                {
                    var debit = new DebitDetail { Id = Guid.NewGuid().ToString(), ApplicationUserId = bill.ApplicationUserId, TimeRecord = bill.DateCreate};
                }
                else // nếu tháng lập hóa đơn đã tồn tại trước rồi thì first debit = firstdebit cũ của tháng đó, incurred = incurred cũ + thêm hóa đơn ; last debit = first debit + incurred debit
                {

                }    
                
                //_db.DebitDetails.Add()
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Create(string[] product,int[] qty,string customer,float total_amount,string check_debit,DateTime time_create) // check_debit : có check là on , uncheck là null
        {
            Bill bill = new Bill();
            try
            {
                List<BillDetail> billDetails = new List<BillDetail>();
                var listbilldetail_update = Get_List_Bill_Detail_Standardized_Quantity(product, qty);
                for (int i = 0; i < listbilldetail_update.Count; i++)
                {
                    billDetails.Add(new BillDetail
                    {
                        BillId = bill.Id,
                        BookId = listbilldetail_update[i].BookId,
                        Count = listbilldetail_update[i].Count,
                        Book = _db.Books.AsNoTracking().FirstOrDefault(x => x.Id == listbilldetail_update[i].BookId)
                    });
                }

                bill.BillDetail = billDetails;
                var cus = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == customer);
                bill.ApplicationUser = cus;
                bool isdebit = false;
                if (check_debit == "on")
                    isdebit = true;
                bill.IsDebit = isdebit;
                if (product.Contains(null))
                {
                    throw new Exception("Không được để hàng trống");
                }
                if (customer == null)
                {
                    throw new Exception("Vui lòng chọn user");
                }
                //get current userId
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                //get current userId
                var user = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == currentUserID);

                


                
                //bill = new Bill() { ApplicationUserId = cus.Id, ApplicationUser = cus, Id = Guid.NewGuid().ToString(), Staff = user, StaffId = user.Id, DateCreate = DateTime.Now, TotalPrice = total_amount, IsDebit = isdebit };
                bill.ApplicationUserId = cus.Id;
                
                bill.Id = Guid.NewGuid().ToString();
                bill.Staff = user;
                bill.StaffId = user.Id;
                bill.DateCreate = time_create;
                bill.TotalPrice = total_amount;


                check_rule_2(bill);
                HttpContext.Session.SetObject("bill", bill);
                return RedirectToAction("BillDemo");
            }
            
            catch (Exception e)
            {

                ModelState.AddModelError("", e.Message);
                Add_SelectList_For_ViewBag();
                return View(bill);
                
            }

            
        }


        public IActionResult GetBookPrice(string id)
        {
            
                var book = _db.Books.Find(id);
                if(book!=null)
                {
                    return Json(new { success = true, price = book.Price });
                }
                return Json(new { success = false, price = 0 });
            
        }

        public IActionResult Create(Bill bill)
        {
            if (bill == null)
                bill = new Bill();
            Add_SelectList_For_ViewBag();
            return View(bill);
        }
        private void Add_SelectList_For_ViewBag()
        {
            ViewBag.Books = new SelectList(_db.Books.ToList(), "Id", "Name");
            var customerlist = _usermanager.GetUsersInRoleAsync("Customer").Result;
            ViewBag.Customer = new SelectList(customerlist, "Id", "FullName");
        }
        private bool IsAvailableTimeRecord(string userid,DateTime timerecord)
        { 
            var list_debit_detail = _db.DebitDetails.ToList();
            foreach(var item in list_debit_detail)
            { // nếu trong danh sách debit detail có 
                if (item.TimeRecord.ToString("yyyy-MM") == timerecord.ToString("yyyy-MM") && item.ApplicationUserId == userid )
                    return true;
            }
            return false;
        }
        private void check_rule_2(Bill bill)
        {
            var rule = _db.Rules.Find("QD2");
            if (rule.UseThisRule == true)
            {
                foreach (var item in bill.BillDetail)
                {
                    if (_db.Books.Find(item.BookId).Quantity - item.Count - rule.Max < 0) // số lượng tồn sau khi bán của sách nếu có sử dụng rule 2 mà < số lượng tồn sau khi bán(quy định) thì sẽ văng ra lỗi
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" vi phạm quy định số hai" + ",số lượng tồn kho của sách sau khi bán phải từ " + rule.Max + " quyển trở lên (Chỉ có thể mua tối đa " + (_db.Books.Find(item.BookId).Quantity - rule.Max) + " quyển)" );
                    }
                }
                if (bill.ApplicationUser.Dept > rule.Min) // nếu nợ của khách lớn hơn quy định cho phép
                    throw new Exception("Chỉ bán cho khách hàng có nợ không  quá" + rule.Min + "đồng");
                else if (bill.IsDebit == true && (bill.ApplicationUser.Dept + bill.TotalPrice > rule.Min)) // nếu nợ của khách nhỏ hơn quy định cho phép nhưng khách lại muốn tiếp tục nợ mà tiền sách nợ + với nợ cũ lớn hơn quy định cho phép thì cũng không bán
                    throw new Exception("Khách hàng hiện đang nợ " + bill.ApplicationUser.Dept + " đồng. Nếu tiếp tục mua nợ thì nợ của khách hàng sẽ là " + (bill.ApplicationUser.Dept + bill.TotalPrice) + " đồng. Để tiếp tục giao dịch, Vui lòng thanh toán nợ cho thủ thư,hoặc chọn phương thức mua không nợ");
             }
            else // nếu không sử dụng rule 2
            {
                // kiểm tra xem số lượng đặt mua có lớn hơn só lượng tồn của kho sách không
                foreach(var item in bill.BillDetail)
                {
                    if(item.Count > _db.Books.Find(item.BookId).Quantity)
                    {
                        throw new Exception("Cuốn sách có tên \" " + item.Book.Name + " \" có số lượng tồn kho không đủ để thực hiện tạo hóa đơn, ( Chỉ có thể mua tối đa " + _db.Books.Find(item.BookId).Quantity + " quyển )");
                    }
                }    
            }
        }
        private List<BillDetail> Get_List_Bill_Detail_Standardized_Quantity(string[] product, int[] quantity)
        {
            List<BillDetail> listbilldetail = new List<BillDetail>();
            for (int i = 0; i < product.Length; i++)
            {
                BillDetail billdetail = new BillDetail() {BookId = product[i], Count = quantity[i] }; 
                listbilldetail.Add(billdetail);
            } // tao ra 1 list product tu 2 mang product va quantity
            var newlistbilldetail = listbilldetail.GroupBy(x => x.BookId)
                .Select(x => new
                {
                    bookid = x.Key,
                    count = x.Sum(y => y.Count)
                }).ToList();

            var listbilldetail_update = newlistbilldetail.Select(x => new BillDetail { BookId = x.bookid, Count = x.count }).ToList();
            return listbilldetail_update;
        }
        private float Find_First_Debit_Of_User(Bill bill)
        {
            var list_debit_detail = _db.DebitDetails.ToList();
            DateTime nearest_date = DateTime.Parse("01-01-0001");
            string debit_detail_id = "";
            foreach(var item in list_debit_detail)
            {
                // tim id debit detail co ngay gan voi ngay hien tai nhat
                if (item.ApplicationUserId == bill.ApplicationUserId && ((item.TimeRecord - nearest_date).TotalDays >0))
                {
                    debit_detail_id = item.Id;
                }
            }
            var first_debit = _db.DebitDetails.FirstOrDefault(x => x.Id == debit_detail_id).LastDebit;
            return first_debit;
        }
    }
}
