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
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
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

        // Onaylanan siparişlerimi görüntüle
        public ActionResult ConfirmedOrders()
        {
            string email = Session["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var confirmedOrders = db.Orders
                                     .Where(o => o.UserEmail == email && o.Status == "Onaylı") // Onaylı siparişler
                                     .ToList();

            return View(confirmedOrders); // View'e Onaylı sipariş listesi dönüyoruz
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

        // Profil sayfası
        public ActionResult Profile()
        {
            string email = Session["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return RedirectToAction("Login");

            return View(user); // Profil bilgilerini Profile view'ına gönder
        }
    }
}
