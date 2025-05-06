using System;
using System.Collections.Generic;

namespace ECommerceMvcSite.Models
{
    public class CancelledOrder
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime CancelDate { get; set; }

        public virtual List<OrderItem> Items { get; set; }
    }
}
