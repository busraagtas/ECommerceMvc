using ECommerceMvcSite.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


//mail göndermek için 
using System.Net;
using System.Net.Mail;
using System.Web;
using System.IO;
using System.Collections.Generic;
using ECommerceMvcSite.Controllers;


public class AdminController : BaseController
{
    private MyDbContext db = new MyDbContext();

    // Admin kontrolü
    private bool IsAdmin()
    {
        return Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
    }

    // Ürün ekleme sayfası (GET)
    public ActionResult AddProduct()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");
        var categories = db.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(); // Views/Admin/AddProduct.cshtml
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddProduct(Product product, HttpPostedFileBase imageFile)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            // Görseli kaydet
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/Products"), fileName);
                imageFile.SaveAs(path);

                product.ImageUrl = "/Images/Products/" + fileName;
            }

            // Veritabanına ekle (örnek)
            db.Products.Add(product);
            db.SaveChanges();

            TempData["Message"] = "Ürün başarıyla eklendi.";
            return RedirectToAction("ProductList"); // veya liste sayfanın adı neyse
        }
        var categories = db.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product); // Hatalıysa aynı sayfada kal
    }


    // Admin Paneli: Ürün listeleme
    public ActionResult ProductList()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var products = db.Products.ToList();
        return View(products); // Views/Admin/ProductList.cshtml
    }

    public ActionResult OrderList()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var orders = db.Orders
            .Include(o => o.Items.Select(i => i.Product)) // Ürünleri de dahil et
            .ToList();

        foreach (var order in orders)
        {
            // Eğer OrderItem içinde Price varsa oradan al, yoksa Product.Price'tan al
            order.TotalPrice = order.Items.Sum(item =>
                item.Price > 0 ? item.Quantity * item.Price : item.Quantity * item.Product.Price);
        }

        return View(orders);
    }

    // Siparişi onaylama
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ApproveOrder(int orderId)
    {
        var order = db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            order.Status = "Onaylandı";
            db.SaveChanges();

            var approvedOrder = new ApprovedOrder
            {
                OrderId = order.Id,
                ApprovedDate = DateTime.Now
            };
            db.ApprovedOrders.Add(approvedOrder);

            foreach (var item in order.Items)
            {
                item.CancelledOrderId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();
            TempData["Message"] = "Sipariş onaylandı.";
        }
       // E-posta gönder
        try
        {
            var fromAddress = new MailAddress("masakioyuncak@gmail.com", "Masakı Oyuncak");
            var toAddress = new MailAddress(order.UserEmail);
            const string fromPassword = "oypp wvsd ipyk whlt"; // Gmail için özel uygulama şifresi gerekir SMTP 
            const string subject = "Siparişiniz Onaylandı";
            string body = $"Merhaba,\n\n{orderId} numaralı siparişiniz onaylanmıştır. En kısa sürede hazırlanıp kargoya verilecektir.\n\nBizi tercih ettiğiniz için teşekkür ederiz.✨";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        catch (Exception ex)
        {
            // Loglama yapılabilir
            TempData["Message"] = "Sipariş onaylandı ancak mail gönderilemedi: " + ex.Message;
        }

        return RedirectToAction("OrderList");


    }
    // GET
    public ActionResult EditProduct(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var product = db.Products.Find(id);
        if (product == null)
        {
            return HttpNotFound();
        }

        var categories = db.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId); // product.CategoryId seçili olacak

        return View(product);
    }


    // Ürün düzenleme işlemi (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditProduct(Product model, HttpPostedFileBase imageFile)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            var product = db.Products.Find(model.Id);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Description = model.Description;

            // Yeni kategori ataması
            product.CategoryId = model.CategoryId;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                imageFile.SaveAs(path);
                product.ImageUrl = "~/Images/" + fileName;
            }

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("ProductList");
        }

        // ModelState hatalıysa kategori listesini tekrar yükle
        var categories = db.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);

        return View(model);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CancelOrderByAdmin(int orderId)
    {
        var order = db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId);

        if (order != null && order.Status == "Hazırlanıyor")
        {
            // 1. Yeni CancelledOrder kaydı oluştur
            var cancelledOrder = new CancelledOrder
            {
                UserEmail = order.UserEmail,
                CancelDate = DateTime.Now,
                Status = "İptal Edildi",
                Items = new List<OrderItem>()  // bu boş da olabilir, EF zaten ilişkilendirir
            };

            db.CancelledOrders.Add(cancelledOrder);
            db.SaveChanges(); // ID burada oluşur

            // 2. Order statusünü güncelle
            order.Status = "Satıcı tarafından iptal edildi";
            order.IsCancelled = true;


            // 3. Her OrderItem'a CancelledOrderId ata
            foreach (var item in order.Items)
            {
                item.CancelledOrderId = cancelledOrder.Id;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();
            TempData["Message"] = "Sipariş iptal edildi.";
        }

        return RedirectToAction("OrderList");
    }
    public ActionResult DeleteProduct(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var product = db.Products.Find(id);
        if (product == null)
        {
            return HttpNotFound();
        }

        db.Products.Remove(product);
        db.SaveChanges();

        TempData["Message"] = "Ürün başarıyla silindi.";
        return RedirectToAction("ProductList");
    }

    // Admin çıkışı
    public ActionResult Logout()
    {
        Session["IsAdmin"] = null;
        return RedirectToAction("Login", "Account");
    }
}
