// OrderItem.cs
using ECommerceMvcSite.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int? OrderId { get; set; }

    [ForeignKey("CancelledOrder")]
    public int? CancelledOrderId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; }
    public virtual CancelledOrder CancelledOrder { get; set; }
    public virtual Product Product { get; set; }
}
