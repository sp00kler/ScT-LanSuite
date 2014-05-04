namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeatingContentLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Seatings", "Content", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Seatings", "Content", c => c.String(nullable: false, maxLength: 4000));
        }
    }
}
