namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabaseAgain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CancelledOrders", "Status", c => c.String());
            AddColumn("dbo.Orders", "Status", c => c.String());
            DropColumn("dbo.Orders", "IsCancelled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Orders", "Status");
            DropColumn("dbo.CancelledOrders", "Status");
        }
    }
}
