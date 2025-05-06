using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMvcSite.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = "/Content/deneme.png";

        // Eğer stok miktarı eklemek isterseniz:
        public int StockQuantity { get; set; }
    }
}
