using ECommerceMvcSite.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }

    // Sipariş ID
    [ForeignKey("Order")]
    public int? OrderId { get; set; }

    // İptal edilen sipariş ID
    [ForeignKey("CancelledOrder")]
    public int? CancelledOrderId { get; set; }

    // Ürün ID
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    // Sipariş miktarı
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Ürünün fiyatı

    // İlişkiler
    public virtual Order Order { get; set; }
    public virtual CancelledOrder CancelledOrder { get; set; }
    public virtual Product Product { get; set; }
}
