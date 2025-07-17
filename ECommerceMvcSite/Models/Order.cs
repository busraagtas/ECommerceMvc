using System.Collections.Generic;
using System;
namespace ECommerceMvcSite.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }


        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
        public bool IsCancelled { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
    }
}
