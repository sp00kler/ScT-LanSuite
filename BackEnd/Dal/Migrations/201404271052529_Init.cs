namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clans",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Tag = c.String(nullable: false, maxLength: 10),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        ClanID = c.String(maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Clan_ID = c.String(maxLength: 128),
                        Team_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clans", t => t.ClanID)
                .ForeignKey("dbo.Clans", t => t.Clan_ID)
                .ForeignKey("dbo.Teams", t => t.Team_ID)
                .Index(t => t.ClanID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Clan_ID)
                .Index(t => t.Team_ID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ClanInvitations",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        ClanID = c.String(maxLength: 128),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clans", t => t.ClanID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.ClanID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ClanSeatings",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        TableName = c.String(nullable: false, maxLength: 100),
                        SeatName = c.String(nullable: false, maxLength: 100),
                        ClanID = c.String(maxLength: 128),
                        EditionID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clans", t => t.ClanID)
                .ForeignKey("dbo.Editions", t => t.EditionID)
                .Index(t => t.ClanID)
                .Index(t => t.EditionID);
            
            CreateTable(
                "dbo.Editions",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Seats = c.Int(nullable: false),
                        isActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Seatings",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Content = c.String(nullable: false, maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Editions", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false, maxLength: 100),
                        Content = c.String(nullable: false, maxLength: 4000),
                        Date = c.DateTime(nullable: false),
                        NewsID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.News", t => t.NewsID)
                .Index(t => t.NewsID);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Content = c.String(nullable: false, maxLength: 4000),
                        Date = c.DateTime(nullable: false),
                        Place = c.Int(nullable: false),
                        PageID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pages", t => t.PageID, cascadeDelete: true)
                .Index(t => t.PageID);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Content = c.String(maxLength: 4000),
                        Place = c.Int(nullable: false),
                        isNews = c.Boolean(nullable: false),
                        isHome = c.Boolean(nullable: false),
                        isActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Compoes",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Rules = c.String(nullable: false, maxLength: 500),
                        Game = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 100),
                        Tag = c.String(nullable: false, maxLength: 10),
                        CompoID = c.String(maxLength: 128),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Compoes", t => t.CompoID)
                .Index(t => t.CompoID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Registrations",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Paid = c.Boolean(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditionID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Editions", t => t.EditionID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.EditionID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Registrations", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Registrations", "EditionID", "dbo.Editions");
            DropForeignKey("dbo.AspNetUsers", "Team_ID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "CompoID", "dbo.Compoes");
            DropForeignKey("dbo.Teams", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.News", "PageID", "dbo.Pages");
            DropForeignKey("dbo.Comments", "NewsID", "dbo.News");
            DropForeignKey("dbo.ClanSeatings", "EditionID", "dbo.Editions");
            DropForeignKey("dbo.Seatings", "ID", "dbo.Editions");
            DropForeignKey("dbo.ClanSeatings", "ClanID", "dbo.Clans");
            DropForeignKey("dbo.AspNetUsers", "Clan_ID", "dbo.Clans");
            DropForeignKey("dbo.Clans", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClanInvitations", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClanInvitations", "ClanID", "dbo.Clans");
            DropForeignKey("dbo.AspNetUsers", "ClanID", "dbo.Clans");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Registrations", new[] { "EditionID" });
            DropIndex("dbo.Registrations", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "CompoID" });
            DropIndex("dbo.News", new[] { "PageID" });
            DropIndex("dbo.Comments", new[] { "NewsID" });
            DropIndex("dbo.Seatings", new[] { "ID" });
            DropIndex("dbo.ClanSeatings", new[] { "EditionID" });
            DropIndex("dbo.ClanSeatings", new[] { "ClanID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.ClanInvitations", new[] { "UserID" });
            DropIndex("dbo.ClanInvitations", new[] { "ClanID" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Team_ID" });
            DropIndex("dbo.AspNetUsers", new[] { "Clan_ID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "ClanID" });
            DropIndex("dbo.Clans", new[] { "UserID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Registrations");
            DropTable("dbo.Teams");
            DropTable("dbo.Compoes");
            DropTable("dbo.Pages");
            DropTable("dbo.News");
            DropTable("dbo.Comments");
            DropTable("dbo.Seatings");
            DropTable("dbo.Editions");
            DropTable("dbo.ClanSeatings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.ClanInvitations");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Clans");
        }
    }
}
