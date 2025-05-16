using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceMvcSite.Models;
using System.Data.Entity;
using System.Net;

namespace ECommerceMvcSite.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();


        [Authorize]
        public ActionResult Siparislerim()
        {
            var testOrder = new Order
            {
                Id = 1,
                OrderDate = DateTime.Now,
                UserEmail = "test@example.com",
                IsCancelled = false,
                Items = new List<OrderItem>
        {
            new OrderItem
            {
                Quantity = 2,
                Product = new Product
                {
                    Name = "Test Ürünü",
                    ImageUrl = "~/images/test.jpg"
                }
            }
        }
            };

            var orders = new List<Order> { testOrder };
            return View(orders);
        }

        [HttpPost]
        public ActionResult IptalEt(int orderId)
        {
            // Örnek DbContext adı: db
            var order = db.Orders.FirstOrDefault(o => o.Id == orderId);

            // Kullanıcı doğrulama (opsiyonel ama önerilir)
            if (order == null || order.IsCancelled)
            {
                return RedirectToAction("Siparislerim");
            }

            // Güvenlik: Sadece kendi siparişini iptal etsin
            var currentUserEmail = User.Identity.Name;
            if (order.UserEmail != currentUserEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            order.IsCancelled = true;
            db.SaveChanges();

            return RedirectToAction("Siparislerim");
        }

    }
}
