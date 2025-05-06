using ECommerceMvcSite.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int ProductId { get; set; } // ✅ BU SATIRI EKLE

    public virtual Product Product { get; set; }

    public int Quantity { get; set; }
}
