namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_FlowFlowProperty_FlowProperty_Fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FlowProperty", "FlowProperty_FlowPropertyID", "dbo.FlowProperty");
            DropIndex("dbo.FlowProperty", new[] { "FlowProperty_FlowPropertyID" });
            DropColumn("dbo.FlowProperty", "FlowProperty_FlowPropertyID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FlowProperty", "FlowProperty_FlowPropertyID", c => c.Int());
            CreateIndex("dbo.FlowProperty", "FlowProperty_FlowPropertyID");
            AddForeignKey("dbo.FlowProperty", "FlowProperty_FlowPropertyID", "dbo.FlowProperty", "FlowPropertyID");
        }
    }
}
