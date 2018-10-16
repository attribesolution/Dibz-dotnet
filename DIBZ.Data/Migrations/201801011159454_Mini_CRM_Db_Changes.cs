namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mini_CRM_Db_Changes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyQueries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        PhoneNo = c.String(),
                        Subject = c.String(),
                        Message = c.String(),
                        QueryStatus = c.Int(nullable: false),
                        AppUserId = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MyQueryDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MyQueryId = c.Int(nullable: false),
                        AdminId = c.Int(),
                        Message = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Admins", t => t.AdminId)
                .ForeignKey("dbo.MyQueries", t => t.MyQueryId, cascadeDelete: true)
                .Index(t => t.MyQueryId)
                .Index(t => t.AdminId);
            
            AddColumn("dbo.ApplicationUsers", "MyQueries_Id", c => c.Int());
            CreateIndex("dbo.ApplicationUsers", "MyQueries_Id");
            AddForeignKey("dbo.ApplicationUsers", "MyQueries_Id", "dbo.MyQueries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MyQueryDetails", "MyQueryId", "dbo.MyQueries");
            DropForeignKey("dbo.MyQueryDetails", "AdminId", "dbo.Admins");
            DropForeignKey("dbo.ApplicationUsers", "MyQueries_Id", "dbo.MyQueries");
            DropIndex("dbo.MyQueryDetails", new[] { "AdminId" });
            DropIndex("dbo.MyQueryDetails", new[] { "MyQueryId" });
            DropIndex("dbo.ApplicationUsers", new[] { "MyQueries_Id" });
            DropColumn("dbo.ApplicationUsers", "MyQueries_Id");
            DropTable("dbo.MyQueryDetails");
            DropTable("dbo.MyQueries");
        }
    }
}
