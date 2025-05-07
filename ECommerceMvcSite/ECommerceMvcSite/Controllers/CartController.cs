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
            var cart = (List<int>)Session["Cart"];
            if (cart == null)
            {
                cart = new List<int>();
                Session["Cart"] = cart;
            }

            var products = db.Products.Where(p => cart.Contains(p.Id)).ToList();

            // Sipariş mesajı varsa ViewBag'e taşı
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(products);
        }

        public ActionResult AddToCart(int productId)
        {
            var cart = (List<int>)Session["Cart"];
            if (cart == null)
            {
                cart = new List<int>();
                Session["Cart"] = cart;
            }

            if (!cart.Contains(productId))
            {
                cart.Add(productId);
            }

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int productId)
        {
            var cart = (List<int>)Session["Cart"];
            if (cart != null)
            {
                cart.Remove(productId);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<int>;
            var userEmail = User.Identity.Name; // Giriş yapan kullanıcının emaili

            if (cart == null || !cart.Any())
            {
                TempData["Message"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Cart");
            }

            // Sepetteki ürünleri getir
            var products = db.Products.Where(p => cart.Contains(p.Id)).ToList();

            // Sipariş oluştur
            var order = new Order
            {
                UserEmail = userEmail,
                OrderDate = DateTime.Now,
                Items = products.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = 1 // Sabit 1 adet olarak kayıt edilir
                }).ToList()
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Sepeti temizle
            Session["Cart"] = null;

            // Sipariş alındı mesajı
            TempData["Message"] = "Siparişiniz başarıyla alındı.";

            return RedirectToAction("Index", "Cart");
        }
    }
}
