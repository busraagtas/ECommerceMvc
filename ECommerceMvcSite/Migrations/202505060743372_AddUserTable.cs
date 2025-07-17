namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Admins", newName: "Users");
            AddColumn("dbo.Users", "IsAdmin", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Users", "Username", c => c.String(nullable: false, maxLength: 50));
            DropTable("dbo.ProductInfoForms");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductInfoForms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(),
                        Message = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Users", "Username", c => c.String(nullable: false));
            DropColumn("dbo.Users", "IsAdmin");
            RenameTable(name: "dbo.Users", newName: "Admins");
        }
    }
}
