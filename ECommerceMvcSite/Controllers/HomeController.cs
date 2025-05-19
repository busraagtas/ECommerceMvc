using System;
using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class HomeController : BaseController
    {
        private MyDbContext _context = new MyDbContext();

        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(products);
        }

        public ActionResult Hakkimizda()  
        {
            ViewBag.Message = "Biz kimiz? Neler yapıyoruz?";
            return View();
        }//pjddjsdzpfwjdpszl

      /*  public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
      */
        public ActionResult Iletisim()
        {
            ViewBag.Message = "Bize ulaşın!";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iletisim(string name, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Message = "Lütfen tüm alanları doldurduğunuzdan emin olun!";
                return View();
            }
            var userEmail = Session["UserEmail"] as string;
            var userId = Session["UserId"] as int?;
            var newMessage = new Message
            {
                UserName = name,
                UserEmail = userEmail,
                RecipientEmail = userEmail, // Cevap bu maile gitsin
                UserId = userId.Value,
                Content = message,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(newMessage);
            _context.SaveChanges();

            ViewBag.Message = "Mesajınız başarıyla gönderildi!";
            return View();
        }


        public ActionResult BizeUlasin()
        {
            // Bize ulaşın sayfasına giden kullanıcıyı iletişim sayfasına yönlendir
            return RedirectToAction("Iletisim");
        }

    }
}
