using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceMvcSite.Models;
using System.Data.Entity;

namespace ECommerceMvcSite.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

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

            return View("Confirmed", confirmedOrders);
        }

        public ActionResult CancelledOrders()
        {
            var userEmail = Session["UserEmail"]?.ToString();
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var cancelledOrders = db.CancelledOrders
                .Where(c => c.UserEmail == userEmail)
                .Include(c => c.Items.Select(i => i.Product))
                .ToList();

            return View("CancelledOrders", cancelledOrders);
        }
    }
}
