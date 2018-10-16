namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailTemplate_AND_EmailNotification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tiltle = c.String(),
                        Body = c.String(),
                        ApplicationUserEmail = c.String(),
                        EmailType = c.Int(nullable: false),
                        IsSend = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailType = c.Int(nullable: false),
                        EmailContentType = c.Int(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailTemplates");
            DropTable("dbo.EmailNotifications");
        }
    }
}
