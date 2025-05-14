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
        public DbSet<ApprovedOrder> ApprovedOrders { get; set; }

        // OnModelCreating metodu
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OrderItem ile Order ilişkisini yapılandır
            modelBuilder.Entity<OrderItem>()
                .HasOptional(oi => oi.Order)
                .WithMany()
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem ile CancelledOrder ilişkisini yapılandır
            modelBuilder.Entity<OrderItem>()
                .HasOptional(oi => oi.CancelledOrder)
                .WithMany()
                .HasForeignKey(oi => oi.CancelledOrderId);

            // ApprovedOrder ile Order ilişkisini yapılandır
            modelBuilder.Entity<ApprovedOrder>()
                .HasRequired(ao => ao.Order)
                .WithMany()
                .HasForeignKey(ao => ao.OrderId)
                .WillCascadeOnDelete(false); // Onaylanan siparişlerin silinmesini engelle
        }
    }
}
