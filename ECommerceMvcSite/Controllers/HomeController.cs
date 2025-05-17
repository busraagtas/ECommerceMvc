using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class HomeController : Controller
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
        public ActionResult Contact(string name, string email, string message)
        {
            if (ModelState.IsValid)
            {
                // Burada e-posta gönderme işlemi yapılabilir veya formu veritabanına kaydedebilirsiniz
                ViewBag.Message = "Mesajınız başarıyla gönderildi!";
                return View();
            }

            // Hatalı giriş durumunda mesaj
            ViewBag.Message = "Lütfen tüm alanları doldurduğunuzdan emin olun!";
            return View();
        }
        public ActionResult BizeUlasin()
        {
            // Bize ulaşın sayfasına giden kullanıcıyı iletişim sayfasına yönlendir
            return RedirectToAction("Iletisim");
        }

    }
}
