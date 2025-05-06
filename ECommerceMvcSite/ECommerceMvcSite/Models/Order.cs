using System;
using System.Collections.Generic;

namespace ECommerceMvcSite.Models
{
    public class Order
    {
        public int Id { get; set; } // ✅ BU SATIR GEREKLİ
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsCancelled { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
    }
}
