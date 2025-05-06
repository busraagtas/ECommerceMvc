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
            var cart = (List<int>)Session["Cart"];
            string userEmail = Session["UserEmail"]?.ToString(); // Kullanıcı oturumu kontrolü

            if (cart == null || !cart.Any() || userEmail == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var products = db.Products.Where(p => cart.Contains(p.Id)).ToList();

            var orderItems = products.Select(p => new OrderItem
            {
                ProductId = p.Id,
                Quantity = 1
            }).ToList();

            var order = new Order
            {
                UserEmail = userEmail,
                OrderDate = DateTime.Now,
                Items = orderItems
            };

            db.Orders.Add(order);
            db.SaveChanges();

            Session["Cart"] = new List<int>();

            return RedirectToAction("MyOrders", "Account");
        }
    }
}
