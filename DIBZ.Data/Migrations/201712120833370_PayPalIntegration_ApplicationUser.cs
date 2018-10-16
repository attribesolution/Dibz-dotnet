namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayPalIntegration_ApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "ApplicationUserId", c => c.Int());
            CreateIndex("dbo.Transactions", "ApplicationUserId");
            AddForeignKey("dbo.Transactions", "ApplicationUserId", "dbo.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ApplicationUserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Transactions", new[] { "ApplicationUserId" });
            DropColumn("dbo.Transactions", "ApplicationUserId");
        }
    }
}
