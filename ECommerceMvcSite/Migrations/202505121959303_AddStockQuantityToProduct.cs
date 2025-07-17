namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStockQuantityToProduct : DbMigration
    {
        public override void Up()
        {
           // AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
           // DropColumn("dbo.Products", "StockQuantity");
        }
    }
}
