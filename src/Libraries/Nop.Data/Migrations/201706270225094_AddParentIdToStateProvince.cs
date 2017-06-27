namespace Nop.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParentIdToStateProvince : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StateProvince", "ParentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StateProvince", "ParentId");
        }
    }
}
