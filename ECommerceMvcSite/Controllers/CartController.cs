using ECommerceMvcSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace ECommerceMvcSite.Controllers
{
    public class CartController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            // Session'dan sepeti alıyoruz
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Sepetteki ürünleri çekiyoruz (Product detaylarını içeriyor)
            var products = cart.Select(item => item.Product).ToList();

            // Sipariş mesajı varsa ViewBag'e taşı
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(cart);  // Burada CartItem listesi model olarak gönderiyoruz
        }

        public ActionResult AddToCart(int productId)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Sepette mevcut ürünü kontrol et
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                // Eğer ürün zaten sepette varsa miktarı artır
                existingItem.Quantity++;
            }
            else
            {
                // Ürün sepette yoksa veritabanından ürünü bul
                var product = db.Products.Find(productId);
                if (product != null)
                {
                    // Yeni bir CartItem ekliyoruz
                    cart.Add(new CartItem
                    {
                        ProductId = productId,  // Ürünün ID'si
                        Quantity = 1,           // Başlangıçta miktar 1
                        Product = product       // Ürünü ekliyoruz (Product nesnesi)
                    });
                }
            }

            // Sepeti Session'a kaydediyoruz
            Session["Cart"] = cart;

            // Sepet sayfasına yönlendiriyoruz
            return RedirectToAction("Index");
        }


        public ActionResult RemoveFromCart(int productId)
        {
            // Sepeti Session'dan alıyoruz
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Sepetteki ürünü buluyoruz
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                // Ürünü sepetten çıkarıyoruz
                cart.Remove(existingItem);
            }

            // Sepeti güncelliyoruz
            Session["Cart"] = cart;

            // Sepet sayfasına yönlendiriyoruz
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Checkout()
        {
            // Sepeti Session'dan alıyoruz
            var cart = Session["Cart"] as List<CartItem>;
            var userEmail = Session["UserEmail"]?.ToString();

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Message"] = "Lütfen giriş yapınız.";
                return RedirectToAction("Index", "Cart");
            }
            // Sepet boşsa mesaj ver
            if (cart == null || !cart.Any())
            {
                TempData["Message"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Cart");
            }

            // Sepetteki ürünleri alıyoruz
            var products = cart.Select(item => item.Product).ToList();

            // Yeni bir sipariş oluşturuyoruz
            var order = new Order
            {
                UserEmail = userEmail,
                OrderDate = DateTime.Now,
                Status = "Hazırlanıyor",
                IsCancelled = false,
                Items = new List<OrderItem>()
            };

            // Ürünleri siparişe ekliyoruz
            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Order = order
                };
                order.Items.Add(orderItem);
            }

            // Siparişi veritabanına ekliyoruz
            db.Orders.Add(order);
            db.SaveChanges();

            // Sepeti sıfırlıyoruz
            Session["Cart"] = null;
            TempData["Message"] = "Siparişiniz başarıyla alındı.";

            // Sepet sayfasına yönlendiriyoruz
            return RedirectToAction("Index", "Cart");
        }


    }
}
