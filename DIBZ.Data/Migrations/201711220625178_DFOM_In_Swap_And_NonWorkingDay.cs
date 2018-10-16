namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DFOM_In_Swap_And_NonWorkingDay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NonWorkingDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NonWorkingDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        Reason = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Swaps", "GamerOffererDFOM", c => c.String());
            AddColumn("dbo.Swaps", "GamerSwapperDFOM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Swaps", "GamerSwapperDFOM");
            DropColumn("dbo.Swaps", "GamerOffererDFOM");
            DropTable("dbo.NonWorkingDays");
        }
    }
}
