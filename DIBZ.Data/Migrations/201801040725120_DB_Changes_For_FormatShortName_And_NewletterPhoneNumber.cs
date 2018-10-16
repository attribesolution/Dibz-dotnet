namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DB_Changes_For_FormatShortName_And_NewletterPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Formats", "ShortName", c => c.String());
            AddColumn("dbo.NewsLetters", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsLetters", "PhoneNumber");
            DropColumn("dbo.Formats", "ShortName");
        }
    }
}
