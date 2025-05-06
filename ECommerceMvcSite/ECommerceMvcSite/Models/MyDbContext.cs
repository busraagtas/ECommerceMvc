using System.Data.Entity;

namespace ECommerceMvcSite.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CancelledOrder> CancelledOrders { get; set; }
    }
}
