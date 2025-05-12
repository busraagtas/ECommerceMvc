namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ECommerceMvcSite.Models;  // Product sınıfı için gerekli

    internal sealed class Configuration : DbMigrationsConfiguration<ECommerceMvcSite.Models.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false; // Migrations otomatik olarak çalışmasın
        }

        //protected override void Seed(ECommerceMvcSite.Models.MyDbContext context)
        //{
        //    //  Bu metod veritabanı güncellendikten sonra çalışır.

        //    // Eğer Products tablosunda hiç ürün yoksa, örnek ürünler ekleyelim
        //    if (!context.Products.Any())
        //    {
        //        context.Products.AddOrUpdate(
        //            p => p.Name,  // Burada Name alanına göre güncelleme yapar.
        //            new Product { Name = "Ürün 1", Price = 100, Description = "Açıklama 1", ImageUrl = "/Content/deneme.png" },
        //            new Product { Name = "Ürün 2", Price = 200, Description = "Açıklama 2", ImageUrl = "/Content/deneme2.png" }
        //        );

        //        context.SaveChanges();  // Değişiklikleri kaydediyoruz.
        //    }
        //}
    }
}
