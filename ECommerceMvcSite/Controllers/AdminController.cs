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


public class AdminController : Controller
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

        return View(product); // Hatalıysa aynı sayfada kal
    }


    // Admin Paneli: Ürün listeleme
    public ActionResult ProductList()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var products = db.Products.ToList();
        return View(products); // Views/Admin/ProductList.cshtml
    }

    // Siparişlerin görüntülenmesi
    public ActionResult OrderList()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var orders = db.Orders.Include(o => o.Items).ToList();

        // Her bir siparişin TotalPrice değerini hesapla
        foreach (var order in orders)
        {
            order.TotalPrice = order.Items.Sum(item => item.Quantity * item.Price);
        }

        return View(orders); // Siparişleri ve TotalPrice'ı View'a gönder
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
            const string fromPassword = "fwpx nvmj caja anop"; // Gmail için özel uygulama şifresi gerekir
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

        return RedirectToAction("ProductList");


    }


    // Ürün düzenleme sayfası (GET)
    public ActionResult EditProduct(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var product = db.Products.Find(id);
        if (product == null)
        {
            return HttpNotFound();
        }
        return View(product); // Views/Admin/EditProduct.cshtml
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

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(imageFile.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                imageFile.SaveAs(path);
                product.ImageUrl = "~/Images/" + fileName;
            }

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("ProductList");
        }

        return View(model);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CancelOrderByAdmin(int orderId)
    {
        var order = db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId);
        if (order != null && order.Status == "Hazırlanıyor")
        {
            order.Status = "Satıcı tarafından iptal edildi";

            foreach (var item in order.Items)
            {
                item.CancelledOrderId = order.Id;
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
