namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsCancelledToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsCancelled");
        }
    }
}
