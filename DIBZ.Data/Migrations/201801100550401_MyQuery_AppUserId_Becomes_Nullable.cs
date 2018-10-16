namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyQuery_AppUserId_Becomes_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MyQueries", "AppUserId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MyQueries", "AppUserId", c => c.Int(nullable: false));
        }
    }
}
