namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUser_PostalCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "PostalCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "PostalCode");
        }
    }
}
