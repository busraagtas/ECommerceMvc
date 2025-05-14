using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceMvcSite.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }

    }
}