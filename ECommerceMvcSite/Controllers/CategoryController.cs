using ECommerceMvcSite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ECommerceMvcSite.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly MyDbContext _context;

        public CategoryController()
        {
            _context = new MyDbContext();
        }

        public ActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category category, HttpPostedFileBase CategoryImage)
        {
            if (ModelState.IsValid)
            {
                if (CategoryImage != null && CategoryImage.ContentLength > 0)
                {
                    // Fotoğrafı kaydedeceğimiz klasör yolu
                    var fileName = Path.GetFileName(CategoryImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/CategoryImages/"), fileName);

                    // Fotoğrafı kaydet
                    CategoryImage.SaveAs(path);

                    // Kategoriye fotoğraf yolunu ata
                    category.ImageUrl = "/Content/CategoryImages/" + fileName;
                }

                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }


        // GET: Category/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/CategoryImages"), fileName);
                    ImageFile.SaveAs(path);
                    category.ImageUrl = "/Content/CategoryImages/" + fileName;
                }

                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(category);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public PartialViewResult CategoryMenu()
        {
            var categories = _context.Categories.ToList();
            return PartialView("CategoryMenu", categories);
        }

    }
}