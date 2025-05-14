namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Mig2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "CancelledOrderId", "dbo.CancelledOrders");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");

            CreateTable(
                "dbo.ApprovedOrders",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    ApprovedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId);

            AddColumn("dbo.OrderItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderItems", "CancelledOrder_Id", c => c.Int());
            AddColumn("dbo.OrderItems", "Order_Id", c => c.Int());
            AddColumn("dbo.Orders", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));

            CreateIndex("dbo.OrderItems", "Order_Id");

            // BU SATIRI YORUMA ALDIK (zaten varsa tekrar eklemeye çalışmasın)
            // CreateIndex("dbo.OrderItems", "CancelledOrder_Id");
            // AddForeignKey("dbo.OrderItems", "CancelledOrder_Id", "dbo.CancelledOrders", "Id");

            AddForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders");
            // DropForeignKey("dbo.OrderItems", "CancelledOrder_Id", "dbo.CancelledOrders"); // kaldırıldı
            DropForeignKey("dbo.ApprovedOrders", "OrderId", "dbo.Orders");

            DropIndex("dbo.OrderItems", new[] { "Order_Id" });
            // DropIndex("dbo.OrderItems", new[] { "CancelledOrder_Id" }); // kaldırıldı
            DropIndex("dbo.ApprovedOrders", new[] { "OrderId" });

            DropColumn("dbo.Orders", "TotalPrice");
            DropColumn("dbo.OrderItems", "Order_Id");
            DropColumn("dbo.OrderItems", "CancelledOrder_Id");
            DropColumn("dbo.OrderItems", "Price");
            DropTable("dbo.ApprovedOrders");

            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "Id");
            AddForeignKey("dbo.OrderItems", "CancelledOrderId", "dbo.CancelledOrders", "Id");
        }
    }
}
