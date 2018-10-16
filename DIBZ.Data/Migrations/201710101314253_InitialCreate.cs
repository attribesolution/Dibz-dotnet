namespace DIBZ.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AboutMe = c.String(),
                        NickName = c.String(),
                        CellNo = c.String(),
                        YearOfBirth = c.String(),
                        ProfileViewedCounter = c.Int(nullable: false),
                        Address = c.String(),
                        ProfileImageId = c.Int(),
                        ScorecardId = c.Int(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Password = c.String(maxLength: 500),
                        PasswordResetToken = c.String(maxLength: 500),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UploadedFiles", t => t.ProfileImageId)
                .ForeignKey("dbo.Scorecards", t => t.ScorecardId)
                .Index(t => t.ProfileImageId)
                .Index(t => t.ScorecardId);
            
            CreateTable(
                "dbo.GameCatalogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormatId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedBy = c.Int(),
                        GameImageId = c.Int(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Formats", t => t.FormatId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.GameImageId, cascadeDelete: true)
                .Index(t => t.FormatId)
                .Index(t => t.CategoryId)
                .Index(t => t.GameImageId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Formats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UploadedFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(maxLength: 200),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ReturnGameCatalogId = c.Int(),
                        OfferStatus = c.Int(nullable: false),
                        ApplicationUserId = c.Int(nullable: false),
                        GameCatalogId = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameCatalogs", t => t.ReturnGameCatalogId)
                .ForeignKey("dbo.GameCatalogs", t => t.GameCatalogId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ReturnGameCatalogId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.GameCatalogId);
            
            CreateTable(
                "dbo.CounterOffers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OfferId = c.Int(nullable: false),
                        GameCounterOfferWithId = c.Int(nullable: false),
                        CounterOfferPersonId = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CounterOfferPersonId, cascadeDelete: true)
                .ForeignKey("dbo.GameCatalogs", t => t.GameCounterOfferWithId, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .Index(t => t.OfferId)
                .Index(t => t.GameCounterOfferWithId)
                .Index(t => t.CounterOfferPersonId);
            
            CreateTable(
                "dbo.Swaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SwapStatus = c.Int(nullable: false),
                        OfferId = c.Int(nullable: false),
                        GameSwapWithId = c.Int(nullable: false),
                        GameSwapPsersonId = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.GameSwapPsersonId, cascadeDelete: true)
                .ForeignKey("dbo.GameCatalogs", t => t.GameSwapWithId, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .Index(t => t.OfferId)
                .Index(t => t.GameSwapWithId)
                .Index(t => t.GameSwapPsersonId);
            
            CreateTable(
                "dbo.LoginSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(maxLength: 500),
                        Platform = c.String(maxLength: 100),
                        DeviceToken = c.String(maxLength: 500),
                        LastAccessTime = c.DateTime(nullable: false),
                        ApplicationUserId = c.Int(),
                        AdminId = c.Int(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Admins", t => t.AdminId)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.AdminId);
            
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsSuperAdmin = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Password = c.String(maxLength: 500),
                        PasswordResetToken = c.String(maxLength: 500),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Scorecards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.Int(),
                        Proposals = c.Int(nullable: false),
                        NoShows = c.Int(nullable: false),
                        GamesSent = c.Int(nullable: false),
                        TestFails = c.Int(nullable: false),
                        DiscScratched = c.Int(nullable: false),
                        CaseOrInstructionsInPoorCondition = c.Int(nullable: false),
                        GameFailedTesting = c.Int(nullable: false),
                        TestPass = c.Int(nullable: false),
                        DIBz = c.Int(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsFeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        News = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsLetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        AppUserId = c.Int(nullable: false),
                        OfferId = c.Int(nullable: false),
                        Channel = c.Int(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        NotificationBusinessType = c.Int(nullable: false),
                        AdditionalData = c.String(),
                        Status = c.Int(nullable: false),
                        LastError = c.String(),
                        DisplayDateTime = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CommunityMemberId = c.String(),
                        Description = c.String(),
                        CreatedTime = c.String(),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUser_Game",
                c => new
                    {
                        GameCatalog_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GameCatalog_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.GameCatalogs", t => t.GameCatalog_Id, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.GameCatalog_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "ScorecardId", "dbo.Scorecards");
            DropForeignKey("dbo.ApplicationUsers", "ProfileImageId", "dbo.UploadedFiles");
            DropForeignKey("dbo.Offers", "ApplicationUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.LoginSessions", "ApplicationUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.LoginSessions", "AdminId", "dbo.Admins");
            DropForeignKey("dbo.Offers", "GameCatalogId", "dbo.GameCatalogs");
            DropForeignKey("dbo.Swaps", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.Swaps", "GameSwapWithId", "dbo.GameCatalogs");
            DropForeignKey("dbo.Swaps", "GameSwapPsersonId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Offers", "ReturnGameCatalogId", "dbo.GameCatalogs");
            DropForeignKey("dbo.CounterOffers", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.CounterOffers", "GameCounterOfferWithId", "dbo.GameCatalogs");
            DropForeignKey("dbo.CounterOffers", "CounterOfferPersonId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.GameCatalogs", "GameImageId", "dbo.UploadedFiles");
            DropForeignKey("dbo.GameCatalogs", "FormatId", "dbo.Formats");
            DropForeignKey("dbo.GameCatalogs", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.ApplicationUser_Game", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUser_Game", "GameCatalog_Id", "dbo.GameCatalogs");
            DropIndex("dbo.ApplicationUser_Game", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUser_Game", new[] { "GameCatalog_Id" });
            DropIndex("dbo.LoginSessions", new[] { "AdminId" });
            DropIndex("dbo.LoginSessions", new[] { "ApplicationUserId" });
            DropIndex("dbo.Swaps", new[] { "GameSwapPsersonId" });
            DropIndex("dbo.Swaps", new[] { "GameSwapWithId" });
            DropIndex("dbo.Swaps", new[] { "OfferId" });
            DropIndex("dbo.CounterOffers", new[] { "CounterOfferPersonId" });
            DropIndex("dbo.CounterOffers", new[] { "GameCounterOfferWithId" });
            DropIndex("dbo.CounterOffers", new[] { "OfferId" });
            DropIndex("dbo.Offers", new[] { "GameCatalogId" });
            DropIndex("dbo.Offers", new[] { "ApplicationUserId" });
            DropIndex("dbo.Offers", new[] { "ReturnGameCatalogId" });
            DropIndex("dbo.GameCatalogs", new[] { "GameImageId" });
            DropIndex("dbo.GameCatalogs", new[] { "CategoryId" });
            DropIndex("dbo.GameCatalogs", new[] { "FormatId" });
            DropIndex("dbo.ApplicationUsers", new[] { "ScorecardId" });
            DropIndex("dbo.ApplicationUsers", new[] { "ProfileImageId" });
            DropTable("dbo.ApplicationUser_Game");
            DropTable("dbo.Testimonials");
            DropTable("dbo.Notifications");
            DropTable("dbo.NewsLetters");
            DropTable("dbo.NewsFeeds");
            DropTable("dbo.Scorecards");
            DropTable("dbo.Admins");
            DropTable("dbo.LoginSessions");
            DropTable("dbo.Swaps");
            DropTable("dbo.CounterOffers");
            DropTable("dbo.Offers");
            DropTable("dbo.UploadedFiles");
            DropTable("dbo.Formats");
            DropTable("dbo.Categories");
            DropTable("dbo.GameCatalogs");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
