namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserEmail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "Product_Id", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "Product_Id" });
            RenameColumn(table: "dbo.OrderItems", name: "Product_Id", newName: "ProductId");
            AddColumn("dbo.Users", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.OrderItems", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderItems", "ProductId");
            AddForeignKey("dbo.OrderItems", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            AlterColumn("dbo.OrderItems", "ProductId", c => c.Int());
            DropColumn("dbo.Users", "Email");
            RenameColumn(table: "dbo.OrderItems", name: "ProductId", newName: "Product_Id");
            CreateIndex("dbo.OrderItems", "Product_Id");
            AddForeignKey("dbo.OrderItems", "Product_Id", "dbo.Products", "Id");
        }
    }
}
