using System;
using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class AccountController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // Login sayfası
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email; // ✅ Email oturuma eklendi

                if (user.IsAdmin)
                    return RedirectToAction("AdminPanel", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // Kayıt sayfası
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email; // ✅ Email oturuma eklendi

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // Çıkış işlemi
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Siparişlerimi görüntüle
        public ActionResult MyOrders()
        {
            string email = Session["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var orders = db.Orders
                           .Include("Items.Product") // ✅ Items içindeki Product'ı da çekiyoruz
                           .Where(o => o.UserEmail == email)
                           .ToList();

            return View(orders); // View'e Order listesi dönüyoruz

           
        }

        // Sipariş iptali
        public ActionResult CancelOrder(int id)
        {
            var order = db.Orders.Include("Items.Product").FirstOrDefault(o => o.Id == id);
            if (order == null || order.Items == null)
                return HttpNotFound();

            var cancelledOrder = new CancelledOrder
            {
                UserEmail = order.UserEmail,
                CancelDate = DateTime.Now,
                Items = order.Items.Select(i => new OrderItem
                {
                    Product = i.Product,
                    Quantity = i.Quantity
                }).ToList()
            };

            db.CancelledOrders.Add(cancelledOrder);
            db.Orders.Remove(order);
            db.SaveChanges();

            return RedirectToAction("CancelledOrders");
        }

        // İptal edilen siparişleri görüntüle
        public ActionResult CancelledOrders()
        {
            string email = Session["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var cancelledOrders = db.CancelledOrders
                                    .Where(o => o.UserEmail == email) // İptal edilen siparişler
                                    .ToList();

            return View(cancelledOrders); // İptal edilen siparişleri CancelledOrders view'ına gönder
        }
    }
}
