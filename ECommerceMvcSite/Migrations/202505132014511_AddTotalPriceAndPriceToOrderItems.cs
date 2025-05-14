namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTotalPriceAndPriceToOrderItem : DbMigration
    {
        public override void Up()
        {
            // OrderItems tablosuna yeni sütunlar ekle
            AddColumn("dbo.OrderItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderItems", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));

            // Foreign key ilişkisini kur
            AddColumn("dbo.OrderItems", "CancelledOrderId", c => c.Int(nullable: true));
            AddForeignKey("dbo.OrderItems", "CancelledOrderId", "dbo.CancelledOrders", "Id", cascadeDelete: false);
            CreateIndex("dbo.OrderItems", "CancelledOrderId");
        }

        public override void Down()
        {
            // Eğer migration geri alınırsa, eklenen sütunları ve foreign key ilişkisini sil
            DropForeignKey("dbo.OrderItems", "CancelledOrderId", "dbo.CancelledOrders");
            DropIndex("dbo.OrderItems", new[] { "CancelledOrderId" });
            DropColumn("dbo.OrderItems", "CancelledOrderId");
            DropColumn("dbo.OrderItems", "TotalPrice");
            DropColumn("dbo.OrderItems", "Price");
        }
    }

}
