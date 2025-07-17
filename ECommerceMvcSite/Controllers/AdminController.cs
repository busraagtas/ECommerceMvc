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
            .OrderByDescending(o => o.OrderDate)  // En yeni siparişler en üstte
            .Include(o => o.Items.Select(i => i.Product))
            .ToList();

        foreach (var order in orders)
        {
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
            order.Status = "Sipariş Onaylandı";

            db.SaveChanges();

            var approvedOrder = new ApprovedOrder
            {
                OrderId = order.Id,
                ApprovedDate = DateTime.Now
            };
            db.ApprovedOrders.Add(approvedOrder);

            foreach (var item in order.Items)
            {
                // Ürünü bul
                var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    if (product.Stock < 0)
                        product.Stock = 0; // Negatif stok engellenir

                    db.Entry(product).State = EntityState.Modified;
                }

                item.CancelledOrderId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();
            TempData["SuccessMessage"] = "Sipariş onaylandı.";

        }

        // E-posta gönder
        try
        {
            var fromAddress = new MailAddress("masakioyuncak@gmail.com", "Masakı Oyuncak");
            var toAddress = new MailAddress(order.UserEmail);
            const string fromPassword = "oypp wvsd ipyk whlt"; // Gmail için özel uygulama şifresi gerekir
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




    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SiparisiOnayla(int orderId)
    {
        var order = db.Orders
                      .Include(o => o.Items.Select(i => i.Product))
                      .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            TempData["Message"] = "Sipariş bulunamadı.";
            return RedirectToAction("OrderList");
        }

        if (order.Status == "Sipariş Onaylandı")
        {
            TempData["Message"] = "Bu sipariş zaten onaylanmış.";
            return RedirectToAction("OrderList");
        }

        foreach (var item in order.Items)
        {
            if (item.Product.Stock >= item.Quantity)
            {
                item.Product.Stock -= item.Quantity;
            }
            else
            {
                TempData["Message"] = $"{item.Product.Name} ürününde yeterli stok yok.";
                return RedirectToAction("OrderList");
            }
        }

        order.Status = "Sipariş Onaylandı";
        db.SaveChanges();

        TempData["SuccessMessage"] = "Sipariş başarıyla onaylandı ve stoklar güncellendi.";
        return RedirectToAction("OrderList");
    }



    public ActionResult OrderDetails(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var order = db.Orders
            .Include(o => o.Items.Select(i => i.Product))
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return HttpNotFound();
        }

        // Toplam fiyatı hesapla
        order.TotalPrice = order.Items.Sum(item =>
            (item.Price > 0 ? item.Price : item.Product.Price) * item.Quantity);

        return View(order);
    }



    public ActionResult SalesReport(int? year, int? month, string search = "")
    {
        int selectedYear = year ?? DateTime.Now.Year;
        int selectedMonth = month ?? DateTime.Now.Month;

        // Siparişleri yıl ve aya göre filtrele
        var orders = db.Orders
                       .Where(o => o.OrderDate.Year == selectedYear && o.OrderDate.Month == selectedMonth);

        // Sipariş kalemlerini çek
        var orderItemsQuery = orders.SelectMany(o => o.Items);

        // Eğer arama kelimesi varsa, ürün adına göre filtrele (büyük/küçük harf duyarsız)
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearch = search.ToLower();
            orderItemsQuery = orderItemsQuery.Where(oi => oi.Product.Name.ToLower().Contains(lowerSearch));
        }

        // Satış verilerini kategori bazında grupla
        var salesData = orderItemsQuery
            .GroupBy(oi => oi.Product.Category)
            .Select(g => new CategorySalesViewModel
            {
                Category = g.Key,
                TotalQuantity = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.Quantity * oi.Product.Price),
                Products = g.GroupBy(oi => oi.Product)
                            .Select(pg => new ProductSalesViewModel
                            {
                                Product = pg.Key,
                                Quantity = pg.Sum(oi => oi.Quantity)
                            }).ToList()
            })
            .ToList();

        // View'a filtre değerlerini yolla
        ViewBag.SelectedYear = selectedYear;
        ViewBag.SelectedMonth = selectedMonth;
        ViewBag.SearchTerm = search;

        return View(salesData);
    }




}