using System.Linq;
using System.Web.Mvc;
using ECommerceMvcSite.Models;

namespace ECommerceMvcSite.Controllers
{
    public class ProductsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }
        // Yeni: Kategoriye göre ürün listeleme
        public ActionResult ListByCategory(int categoryId)
        {
            var products = db.Products
                           .Where(p => p.CategoryId == categoryId)
                           .ToList();

            return View("Index", products); // aynı Index view'ını kullanabilirsin
        }

    }
}
