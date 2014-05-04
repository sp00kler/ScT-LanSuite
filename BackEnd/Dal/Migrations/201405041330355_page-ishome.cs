namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pageishome : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Pages", "isHome");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pages", "isHome", c => c.Boolean(nullable: false));
        }
    }
}
