namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCancelledOrderSystem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            RenameColumn(table: "dbo.OrderItems", name: "CancelledOrder_Id", newName: "CancelledOrderId");
            RenameIndex(table: "dbo.OrderItems", name: "IX_CancelledOrder_Id", newName: "IX_CancelledOrderId");
            AlterColumn("dbo.OrderItems", "OrderId", c => c.Int());
            CreateIndex("dbo.OrderItems", "OrderId");
            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "Id");
            DropColumn("dbo.Orders", "IsCancelled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            AlterColumn("dbo.OrderItems", "OrderId", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.OrderItems", name: "IX_CancelledOrderId", newName: "IX_CancelledOrder_Id");
            RenameColumn(table: "dbo.OrderItems", name: "CancelledOrderId", newName: "CancelledOrder_Id");
            CreateIndex("dbo.OrderItems", "OrderId");
            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
    }
}
