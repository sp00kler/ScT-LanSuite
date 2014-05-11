namespace Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailconfirmationtoken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ConfirmationToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ConfirmationToken");
        }
    }
}
