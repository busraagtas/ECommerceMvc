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
    public class OrderController : BaseController
    {
        private readonly MyDbContext db = new MyDbContext();

        public ActionResult Siparislerim()
        {
            var email = User.Identity.Name;

            var orders = db.Orders
                .Include("Items.Product")
                .Where(o => o.UserEmail == email && o.Status == "Hazırlanıyor" && !o.IsCancelled)
                .ToList();

            ViewBag.Baslik = "Siparişlerim";
            return View("Siparislerim", orders);
        }

        public ActionResult OnaylananSiparislerim()
        {
            var email = User.Identity.Name;

            var orders = db.Orders
                .Include("Items.Product")
                .Where(o => o.UserEmail == email && o.Status == "Sipariş Onaylandı" && !o.IsCancelled)
                .ToList();

            ViewBag.Baslik = "Onaylanan Siparişlerim";
            return View("Siparislerim", orders);
        }

        public ActionResult IptalEdilenSiparislerim()
        {
            var email = User.Identity.Name;

            var orders = db.Orders
                .Include("Items.Product")
                .Where(o => o.UserEmail == email && o.Status == "Satıcı tarafından iptal edildi" && o.IsCancelled)
                .ToList();

            ViewBag.Baslik = "İptal Edilen Siparişlerim";
            return View("Siparislerim", orders);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IptalEt(int orderId)
        {
            using (var db = new MyDbContext())
            {
                var order = db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId && o.Status == "Hazırlanıyor");

                if (order == null || order.IsCancelled)
                {
                    TempData["Message"] = "Sipariş bulunamadı veya zaten iptal edilmiş.";
                    return RedirectToAction("Siparislerim", "Order");
                }

                order.Status = "İptal Edildi";
                order.IsCancelled = true;

                var cancelledOrder = new CancelledOrder
                {
                    UserEmail = order.UserEmail,
                    CancelDate = DateTime.Now,
                    Status = "İptal Edildi",
                    Items = new List<OrderItem>()
                };

                db.CancelledOrders.Add(cancelledOrder);
                db.SaveChanges();

                foreach (var item in order.Items)
                {
                    item.CancelledOrderId = cancelledOrder.Id;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                TempData["Message"] = "Sipariş başarıyla iptal edildi.";
                return RedirectToAction("MyOrders", "Account", new { tab = "hazirlaniyor" });
            }
        }

    }
}