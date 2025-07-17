namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            RenameColumn(table: "dbo.OrderItems", name: "OrderId", newName: "Order_Id");
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderItems", "Order_Id", c => c.Int());
            CreateIndex("dbo.OrderItems", "Order_Id");
            AddForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders", "Id");
            DropColumn("dbo.Products", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "MyProperty", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "Order_Id" });
            AlterColumn("dbo.OrderItems", "Order_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Products", "StockQuantity");
            DropColumn("dbo.Orders", "IsCancelled");
            RenameColumn(table: "dbo.OrderItems", name: "Order_Id", newName: "OrderId");
            CreateIndex("dbo.OrderItems", "OrderId");
            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
    }
}
