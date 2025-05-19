using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class AccountController : BaseController
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
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        // Giriş Sayfası (POST)
        [HttpPost]
        public ActionResult Login(string email, string password, string ReturnUrl)
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
                Session["UserLastName"] = "Adminella";

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

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl); // Burada önceki sayfaya dönüş yapılır
                }
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
            var email = Session["UserEmail"]?.ToString(); // 🛑 Bu null geliyorsa filtreleme çalışmaz

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account"); // Kullanıcı giriş yapmamış
            }

            var orders = db.Orders
                .Where(o => o.UserEmail == email)
                .Include(o => o.Items.Select(i => i.Product))
                .ToList();

            return View(orders);
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
            var currentUserEmail = Session["UserEmail"]?.ToString();
            if (currentUserEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new MyDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                if (user != null)
                {
                    bool emailChanged = user.Email != updatedUser.Email;
                    bool firstNameSame = user.FirstName == updatedUser.FirstName;
                    bool lastNameSame = user.LastName == updatedUser.LastName;
                    bool nameChanged = !firstNameSame || !lastNameSame;

                    if (!emailChanged)
                    {
                        // Email değişmemişse:
                        if (firstNameSame && lastNameSame)
                        {
                            ViewBag.Message = "Ad veya soyad eskisiyle aynı olamaz, lütfen farklı bir değer girin.";
                            return View("Settings", user);
                        }
                        else
                        {
                            // Sadece ad/soyad değiştiyse ama email aynı
                            user.FirstName = updatedUser.FirstName;
                            user.LastName = updatedUser.LastName;

                            db.SaveChanges();

                            Session.Clear();
                            TempData["UpdateMessage"] = "Bilgileriniz başarıyla güncellendi. Lütfen tekrar giriş yapın.";
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        // Email değiştiyse, ad soyad farketmez, güncelle ve çıkış yap
                        user.FirstName = updatedUser.FirstName;
                        user.LastName = updatedUser.LastName;
                        user.Email = updatedUser.Email;

                        db.SaveChanges();

                        Session.Clear();
                        TempData["UpdateMessage"] = "Bilgileriniz başarıyla güncellendi. Lütfen tekrar giriş yapın.";
                        return RedirectToAction("Login", "Account");
                    }
                }
            }

            ViewBag.Message = "Bir hata oluştu.";
            return View("Settings", updatedUser);
        }


        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var currentUserEmail = Session["UserEmail"]?.ToString();
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

                // ✅ Şifre karşılaştırmaları hashlenmiş şekilde yapılmalı
                string hashedOldPassword = HashPassword(oldPassword);
                string hashedNewPassword = HashPassword(newPassword);

                if (user.Password != hashedOldPassword)
                {
                    ViewBag.PasswordMessage = "Eski şifre yanlış.";
                    return View("Settings", user);
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.PasswordMessage = "Yeni şifreler eşleşmiyor.";
                    return View("Settings", user);
                }

                if (hashedOldPassword == hashedNewPassword)
                {
                    ViewBag.PasswordMessage = "Yeni şifre, eski şifreyle aynı olamaz.";
                    return View("Settings", user);
                }

                user.Password = hashedNewPassword;
                db.SaveChanges();

                Session.Clear();
                TempData["PasswordChanged"] = "Şifreniz başarıyla değiştirildi. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult UpdateAddress(string newAddress)
        {
            var currentUserEmail = Session["UserEmail"]?.ToString();
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
        [HttpPost]
        public ActionResult UpdatePhoneNumber(string newPhoneNumber)
        {
            var email = Session["UserEmail"]?.ToString();
            if (email == null)
                return RedirectToAction("Login", "Account");

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.PhoneMessage = "Kullanıcı bulunamadı.";
                return View("Settings", user);
            }

            user.PhoneNumber = newPhoneNumber;
            db.SaveChanges();

            ViewBag.PhoneMessage = "Telefon numarası başarıyla güncellendi.";
            return View("Settings", user);
        }
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Lütfen e-posta adresinizi girin.";
                return View();
            }

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Bu e-posta adresine kayıtlı kullanıcı bulunamadı.";
                return View();
            }

            // Burada basit olarak kullanıcıya yeni bir şifre veriyoruz.
            // Daha gelişmiş yöntemler: mail ile sıfırlama linki gönderme
            string newPassword = GenerateRandomPassword();
            user.Password = HashPassword(newPassword);
            db.SaveChanges();

            ViewBag.Message = $"Yeni şifreniz: {newPassword} (Lütfen giriş yaptıktan sonra şifrenizi değiştirin.)";

            // İstersen e-posta gönderme kodu da buraya eklenebilir.

            return View();
        }

        // Yardımcı metod: Rastgele şifre üret
        private string GenerateRandomPassword(int length = 8)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(valid, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
