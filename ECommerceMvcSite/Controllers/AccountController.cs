using System;
using System.Data.Entity;
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

            // Admin için sabit kontrol
            if (email == "admin@site.com" && password == "admin1234")
            {
                // Admin girişi
                Session["UserId"] = 0; // Admin için özel bir id
                Session["Username"] = "Admin";
                Session["IsAdmin"] = true;
                Session["UserRole"] = "Admin"; // ✅ BU SATIRI EKLEDİK
                Session["UserEmail"] = email;
                Session["UserFirstName"] = "Admin";
                Session["UserLastName"] = "Admin";

                return RedirectToAction("AddProduct", "Admin"); // Admin paneline yönlendir
            }

            // Normal kullanıcı kontrolü
            var user = db.Users.FirstOrDefault(x => x.Email == email && x.Password == hashedPassword);

            if (user != null)
            {
                // Kullanıcı bilgilerini session'a kaydediyoruz
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserEmail"] = user.Email;
                Session["UserFirstName"] = user.FirstName;
                Session["UserLastName"] = user.LastName;

                // ✅ Rolü burada belirliyoruz
                Session["UserRole"] = user.IsAdmin ? "Admin" : "User";

                if (user.IsAdmin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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

                // Şifreyi hash'leyip veritabanına kaydediyoruz
                user.Password = HashPassword(user.Password);
                db.Users.Add(user);
                db.SaveChanges();

                // Otomatik giriş yapmıyoruz, sadece giriş sayfasına yönlendiriyoruz
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
        public ActionResult ConfirmedOrders()
        {
            var userEmail = Session["UserEmail"]?.ToString();

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var confirmedOrders = db.Orders
                .Where(o => o.UserEmail == userEmail && !o.IsCancelled)
                .Include(o => o.Items.Select(i => i.Product))
                .ToList();

            if (confirmedOrders == null || !confirmedOrders.Any())
            {
                ViewBag.Message = "Henüz onaylı siparişiniz bulunmamaktadır.";
                return View("Confirmed", confirmedOrders);
            }

            return View("ConfirmedOrders", confirmedOrders);
        }
        public ActionResult CancelledOrders()
        {
            var userEmail = Session["UserEmail"]?.ToString(); // Giriş yapan kullanıcının email'i
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            var cancelledOrders = db.Orders
                .Where(o => o.UserEmail == userEmail && o.IsCancelled) // İptal edilen siparişler
                .Include(o => o.Items.Select(i => i.Product)) // Siparişe ait ürünler
                .ToList();

            return View("CancelledOrders", cancelledOrders);
        }

        // [HttpPost]
        public ActionResult Settings()
        {
            // Oturumda kullanıcı ID'sinin olup olmadığını kontrol et
            var userId = Session["UserId"] as int?;

            if (userId == null)
            {
                // Eğer kullanıcı giriş yapmamışsa, giriş sayfasına yönlendir
                return RedirectToAction("Login", "Account");
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return View(user); // Kullanıcı bilgilerini View'a gönder
            }

            // Kullanıcı bulunamazsa, giriş sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult Settings(User updatedUser)
        {
            // Oturumdaki kullanıcı ID'sini al
            var userId = Session["UserId"] as int?;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                // Kullanıcı bilgilerini güncelle
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber; // Telefon numarasını güncelle

                // Değişiklikleri veritabanına kaydet
                db.SaveChanges();

                // Profil sayfasına yönlendir
                return RedirectToAction("Profile", "Account");
            }

            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public ActionResult UpdateUserInfo(User updatedUser)
        {
            var currentUserEmail = Session["Email"]?.ToString();
            if (currentUserEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new MyDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                if (user != null)
                {
                    user.FirstName = updatedUser.FirstName;
                    user.LastName = updatedUser.LastName;
                    user.Email = updatedUser.Email;

                    db.SaveChanges();

                    ViewBag.Message = "Bilgileriniz başarıyla güncellendi.";
                    return View("Settings", user);
                }
            }

            ViewBag.Message = "Bir hata oluştu.";
            return View("Settings", updatedUser);
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var currentUserEmail = Session["Email"]?.ToString();
            if (currentUserEmail == null)
                return RedirectToAction("Login", "Account");

            using (var db = new MyDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == currentUserEmail);

                if (user == null)
                {
                    ViewBag.PasswordMessage = "Kullanıcı bulunamadı.";
                    return View("Settings", user);
                }

                if (user.Password != oldPassword) // Şifreyi hash'liyorsan burada hash karşılaştırması yapılmalı
                {
                    ViewBag.PasswordMessage = "Eski şifre yanlış.";
                    return View("Settings", user);
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.PasswordMessage = "Yeni şifreler eşleşmiyor.";
                    return View("Settings", user);
                }

                user.Password = newPassword;
                db.SaveChanges();

                ViewBag.PasswordMessage = "Şifreniz başarıyla güncellendi.";
                return View("Settings", user);
            }
        }

        [HttpPost]
        public ActionResult UpdateAddress(string newAddress)
        {
            var currentUserEmail = Session["Email"]?.ToString();
            if (currentUserEmail == null)
                return RedirectToAction("Login", "Account");

            using (var db = new MyDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                if (user == null)
                {
                    ViewBag.AddressMessage = "Kullanıcı bulunamadı.";
                    return View("Settings", user);
                }

                user.Address = newAddress;
                db.SaveChanges();

                ViewBag.AddressMessage = "Adres başarıyla güncellendi.";
                return View("Settings", user);
            }
        }


    }
}
