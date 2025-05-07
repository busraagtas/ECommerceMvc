using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceMvcSite.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
