
using ECommerceMvcSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ECommerceMvcSite.Controllers
{
    public class BaseController : Controller
    {
        private MyDbContext db = new MyDbContext();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var userId = Session["UserId"] as int?;
            var userRole = Session["UserRole"] as string;

            int unreadCount = 0;

            if (userId.HasValue && !string.IsNullOrEmpty(userRole))
            {
                if (userRole == "User")
                {
                    // Kullanıcının admin tarafından cevaplanmış ama okumadığı mesaj sayısı
                    unreadCount = db.Messages.Count(m => m.UserId == userId && !m.IsRead && !string.IsNullOrEmpty(m.AdminResponse));
                }
                else if (userRole == "Admin")
                {
                    // Adminin cevap vermesi gereken (AdminResponse boş) mesajlar
                    unreadCount = db.Messages.Count(m => string.IsNullOrEmpty(m.AdminResponse));
                }
            }

            ViewBag.UnreadMessageCount = unreadCount;
        }

    }

}