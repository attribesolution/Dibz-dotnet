namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferDFOM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "GameOffererDFOM", c => c.String());
            AddColumn("dbo.Offers", "GameSwapperDFOM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "GameSwapperDFOM");
            DropColumn("dbo.Offers", "GameOffererDFOM");
        }
    }
}
