﻿namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "StockQuantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
        }
    }
}
