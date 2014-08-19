namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IX_ILCDEntity_UUID_Version : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ILCDEntity", new[] { "UUID", "Version" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.ILCDEntity", new[] { "UUID", "Version" });
        }
    }
}
