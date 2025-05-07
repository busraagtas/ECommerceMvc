using System;
using System.Collections.Generic;

namespace ECommerceMvcSite.Models
{
    public class CancelledOrder
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime CancelDate { get; set; }

        // Siparişin durumu
        public string Status { get; set; }  // Onaylı veya İptal

        public virtual List<OrderItem> Items { get; set; }
    }
}
