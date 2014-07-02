namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_FlowFlowProperty_FlowProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlowProperty", "FlowProperty_FlowPropertyID", c => c.Int());
            CreateIndex("dbo.FlowFlowProperty", "FlowPropertyID");
            CreateIndex("dbo.FlowProperty", "FlowProperty_FlowPropertyID");
            AddForeignKey("dbo.FlowProperty", "FlowProperty_FlowPropertyID", "dbo.FlowProperty", "FlowPropertyID");
            AddForeignKey("dbo.FlowFlowProperty", "FlowPropertyID", "dbo.FlowProperty", "FlowPropertyID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FlowFlowProperty", "FlowPropertyID", "dbo.FlowProperty");
            DropForeignKey("dbo.FlowProperty", "FlowProperty_FlowPropertyID", "dbo.FlowProperty");
            DropIndex("dbo.FlowProperty", new[] { "FlowProperty_FlowPropertyID" });
            DropIndex("dbo.FlowFlowProperty", new[] { "FlowPropertyID" });
            DropColumn("dbo.FlowProperty", "FlowProperty_FlowPropertyID");
        }
    }
}
