namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "NewsID", "dbo.News");
            DropIndex("dbo.Comments", new[] { "NewsID" });
            AlterColumn("dbo.Comments", "NewsID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Comments", "NewsID");
            AddForeignKey("dbo.Comments", "NewsID", "dbo.News", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "NewsID", "dbo.News");
            DropIndex("dbo.Comments", new[] { "NewsID" });
            AlterColumn("dbo.Comments", "NewsID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Comments", "NewsID");
            AddForeignKey("dbo.Comments", "NewsID", "dbo.News", "ID");
        }
    }
}
