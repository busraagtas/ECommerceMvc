namespace ECommerceMvcSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        UserName = c.String(),
                        UserEmail = c.String(),
                        Content = c.String(),
                        SentAt = c.DateTime(nullable: false),
                        AdminResponse = c.String(),
                        ResponseDate = c.DateTime(),
                        RecipientEmail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Messages");
        }
    }
}
