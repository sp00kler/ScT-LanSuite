namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeatingContentRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Seatings", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Seatings", "Content", c => c.String(nullable: false));
        }
    }
}
