namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditionPlace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Editions", "Place", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Editions", "Place");
        }
    }
}
