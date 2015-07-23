namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NodeCacheILCDEntity : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE dbo.NodeCache");
            AddColumn("dbo.Scenario", "Description", c => c.String());
            AddColumn("dbo.NodeCache", "ILCDEntityID", c => c.Int(nullable: true));
            CreateIndex("dbo.NodeCache", "ILCDEntityID");
            AddForeignKey("dbo.NodeCache", "ILCDEntityID", "dbo.ILCDEntity", "ILCDEntityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NodeCache", "ILCDEntityID", "dbo.ILCDEntity");
            DropIndex("dbo.NodeCache", new[] { "ILCDEntityID" });
            DropColumn("dbo.NodeCache", "ILCDEntityID");
            DropColumn("dbo.Scenario", "Description");
        }
    }
}
