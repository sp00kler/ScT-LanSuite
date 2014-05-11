namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cascade3 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Comments", new[] { "NewsID" });
            DropIndex("dbo.News", new[] { "PageID" });
            AlterColumn("dbo.Comments", "NewsID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.News", "PageID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Comments", "NewsID");
            CreateIndex("dbo.News", "PageID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.News", new[] { "PageID" });
            DropIndex("dbo.Comments", new[] { "NewsID" });
            AlterColumn("dbo.News", "PageID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Comments", "NewsID", c => c.String(maxLength: 128));
            CreateIndex("dbo.News", "PageID");
            CreateIndex("dbo.Comments", "NewsID");
        }
    }
}
