using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class AdminController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // Admin yetkisi kontrolü
        private bool IsAdmin()
        {
            return Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
        }

        public ActionResult AdminPanel()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var products = db.Products.ToList();
            return View(products);
        }

        public ActionResult AddProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("AdminPanel");
            }

            return View(product);
        }

        public ActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("AdminPanel");
        }
    }
}

