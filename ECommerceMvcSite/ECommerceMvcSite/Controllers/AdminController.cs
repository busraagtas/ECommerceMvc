using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceMvcSite.Models;



namespace ECommerceMvcSite.Controllers
{
    public class AdminController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // Admin yetkisi kontrolü
        private bool IsAdmin()
        {
            return Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
        }

        public ActionResult AdminPanel()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var products = db.Products.ToList();
            return View(products);
        }

        private static List<Product> products = new List<Product>();

        // Ürün ekleme sayfası
        public ActionResult AddProduct()
        {
            return View();
        }
        public ActionResult ProductList()
        {
            if (Session["IsAdmin"] == null)
                return RedirectToAction("Login");

            return View(products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Fotoğraf dosyasının var olup olmadığını kontrol et
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    try
                    {
                        // Benzersiz bir dosya adı oluşturmak için Guid kullanıyoruz
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                        // Fotoğrafın kaydedileceği yolu belirliyoruz
                        string imagesFolder = Server.MapPath("~/Images"); // Bu path doğru yerde mi?
                        string path = Path.Combine(imagesFolder, fileName);

                        // Eğer Images klasörü yoksa oluştur
                        if (!Directory.Exists(imagesFolder))
                        {
                            Directory.CreateDirectory(imagesFolder); // Klasör gerçekten oluşturuluyor mu?
                        }

                        // Fotoğrafı kaydet
                        imageFile.SaveAs(path); // Fotoğraf kaydediliyor mu?

                        // Fotoğraf yolunu veritabanına kaydet
                        product.ImageUrl = "~/Images/" + fileName; // Yalnızca yolu kaydediyoruz
                    }
                    catch (Exception ex)
                    {
                        // Hata loglama veya mesaj yazdırma işlemi
                        ViewBag.ErrorMessage = "Fotoğraf kaydedilirken bir hata oluştu: " + ex.Message;
                        return View();
                    }
                }

                // Ürün bilgilerini veritabanına kaydet
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Model geçersizse, formu tekrar göster
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);

            if (product != null)
            {
                // Fotoğrafı kaldır (isteğe bağlı, sadece diskteki fotoğrafı silmek için)
                var imagePath = Server.MapPath(product.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Ürünü veritabanından sil
                db.Products.Remove(product);
                db.SaveChanges();
            }

            // Silme işlemi sonrası admin panelindeki ürün listesine geri yönlendir
            return RedirectToAction("Index");
        }


        // GET: Admin/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); // Veya Profile sayfan
            }

            return View(product);
        }

        // GET: Admin/Login
        //public ActionResult Login()
        //{
        //    return View();
        //}

        //// POST: Admin/Login
        //[HttpPost]
        //public ActionResult Login(string username, string password)
        //{
        //    // Sabit admin girişi (veritabanı bağlantısız)
        //    if (username == "admin" && password == "1234")
        //    {
        //        Session["IsAdmin"] = true;
        //        return RedirectToAction("Panel");
        //    }

        //    ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
        //    return View();
        //}

        // GET: Admin/Panel
        public ActionResult Panel()
        {
            if (Session["IsAdmin"] == null)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult Logout()
        {
            Session["IsAdmin"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult Index()
        {
            // Admin paneli için gerekli verileri model olarak gönderebiliriz
            var model = db.Products.ToList();  // Örnek olarak ürün listesini gönderiyorum
            return View(model);
        }

        // Profil sayfası
        public ActionResult Profile()
        {
            // Ürünleri veritabanından alıyoruz
            var products = db.Products.ToList();
            return View(products); // Profil view'ına ürünleri gönderiyoruz
        }
    }
}

