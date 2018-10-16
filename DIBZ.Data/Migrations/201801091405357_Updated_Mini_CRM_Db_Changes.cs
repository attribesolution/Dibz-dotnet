namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_Mini_CRM_Db_Changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MyQueries", "IsDeletedByAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.MyQueries", "IsDeletedByAppUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyQueries", "IsDeletedByAppUser");
            DropColumn("dbo.MyQueries", "IsDeletedByAdmin");
        }
    }
}
