using ECommerceMvcSite.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerceMvcSite.Controllers
{
    public class ProductsController : BaseController
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


        public ActionResult Details(int id)
        {
            var product = db.Products.Include("Category").FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();

            // —— Kullanıcının baktığı ürünleri Session’da sakla ——
            var viewed = Session["ViewedProducts"] as List<int> ?? new List<int>();
            if (!viewed.Contains(id))
                viewed.Add(id);
            Session["ViewedProducts"] = viewed;
            // ——————————————————————————————————————————————

            var related = db.Products
    .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
    .ToList();

            ViewBag.RelatedProducts = related;

            return View(product);
        }


        [HttpGet]
        public JsonResult Search(string term)
        {
            var matchingProducts = db.Products
                .Where(p => p.Name.Contains(term))  // Baştan değil, içinde geçenleri getirir
                .Select(p => new { label = p.Name, value = p.Id })
                .ToList();

            return Json(matchingProducts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Boşsa direkt boş liste gönder
                return View(new List<Product>());
            }

            var results = db.Products
                .Where(p => p.Name.Contains(query))
                .ToList();

            ViewBag.Query = query;
            return View(results); // SearchResults.cshtml dosyasına gönder
        }


        public ActionResult Recommendations()
        {
            // Önce baktığı ürünleri al
            var viewed = Session["ViewedProducts"] as List<int> ?? new List<int>();

            // Eğer hiç bakılan ürün yoksa boş liste döndür
            if (!viewed.Any())
                return PartialView("_Recommendations", new List<Product>());

            // En son baktığı ürünün kategorisine bak
            int lastId = viewed.Last();
            var lastProduct = db.Products.Find(lastId);

            // Aynı kategoriden, daha önce bakılanlar dışındaki 4 ürünü öner
            var recs = db.Products
                         .Where(p => p.CategoryId == lastProduct.CategoryId && !viewed.Contains(p.Id))
                         .Take(4)
                         .ToList();

            return PartialView("_Recommendations", recs);
        }



    }
}