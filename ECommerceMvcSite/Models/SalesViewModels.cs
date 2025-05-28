using System.Collections.Generic;

namespace ECommerceMvcSite.Models
{
    public class CategorySalesViewModel
    {
        public Category Category { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; } // buraya ekledik
        public List<ProductSalesViewModel> Products { get; set; }
    }

    public class ProductSalesViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }


}