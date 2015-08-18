namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Classification : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Classification");
            CreateTable(
    "dbo.Classification",
    c => new
    {
        ClassificationID = c.Int(nullable: false, identity: true),
        CategoryID = c.Int(nullable: false),
        ILCDEntityID = c.Int(nullable: false),
    })
    .PrimaryKey(t => t.ClassificationID)
    .ForeignKey("dbo.Category", t => t.CategoryID)
    .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
    .Index(t => t.CategoryID)
    .Index(t => t.ILCDEntityID);

        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Classification");
            AlterColumn("dbo.Classification", "ClassificationID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Classification", "ClassificationID");
        }
    }
}
