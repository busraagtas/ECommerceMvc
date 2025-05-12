namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveStockQuantityProperly : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
        }
    }
}
