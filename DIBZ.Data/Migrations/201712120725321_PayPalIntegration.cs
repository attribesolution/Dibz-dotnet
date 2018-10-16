namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayPalIntegration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OfferId = c.Int(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        Aomunt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .Index(t => t.OfferId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "OfferId", "dbo.Offers");
            DropIndex("dbo.Transactions", new[] { "OfferId" });
            DropTable("dbo.Transactions");
        }
    }
}
