using System;
using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class AccountController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // Yardımcı fonksiyon: Şifreyi Hash'le
        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Giriş Sayfası (GET)
        public ActionResult Login()
        {
            return View();
        }

        // Giriş Sayfası (POST)
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var user = db.Users.FirstOrDefault(x => x.Email == email && x.Password == hashedPassword);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email;
                Session["UserFirstName"] = user.FirstName;
                Session["UserLastName"] = user.LastName;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
            return View();
        }

        // Kayıt Sayfası (GET)
        public ActionResult Register()
        {
            return View();
        }

        // Kayıt Sayfası (POST)
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ViewBag.Error = "Bu e-posta adresi ile daha önce kayıt olmuş bir kullanıcı var.";
                    return View(user);
                }

                if (db.Users.Any(u => u.Username == user.Username))
                {
                    ViewBag.Error = "Bu kullanıcı adı ile daha önce kayıt olmuş bir kullanıcı var.";
                    return View(user);
                }

                user.Password = HashPassword(user.Password);
                db.Users.Add(user);
                db.SaveChanges();

                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email;
                Session["UserFirstName"] = user.FirstName;
                Session["UserLastName"] = user.LastName;

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // Çıkış İşlemi
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Profil Sayfası
        public ActionResult Profile()
        {
            string email = Session["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        // ✅ Siparişlerim Sayfası
        public ActionResult MyOrders()
        {
            string email = Session["UserEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var orders = db.Orders
                           .Where(o => o.UserEmail == email && !o.IsCancelled)
                           .ToList();

            return View("ConfirmedOrders", orders); // Confirmed.cshtml dosyasını kullan
        }
    }
}
