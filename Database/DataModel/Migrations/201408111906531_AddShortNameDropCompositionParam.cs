namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShortNameDropCompositionParam : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CompositionParam", "FlowFlowPropertyID", "dbo.FlowFlowProperty");
            DropForeignKey("dbo.CompositionParam", "ParamID", "dbo.Param");
            DropIndex("dbo.CompositionParam", new[] { "ParamID" });
            DropIndex("dbo.CompositionParam", new[] { "FlowFlowPropertyID" });
            AddColumn("dbo.FragmentFlow", "ShortName", c => c.String(maxLength: 30));
            DropTable("dbo.CompositionParam");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CompositionParam",
                c => new
                    {
                        CompositionParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        FlowFlowPropertyID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.CompositionParamID);
            
            DropColumn("dbo.FragmentFlow", "ShortName");
            CreateIndex("dbo.CompositionParam", "FlowFlowPropertyID");
            CreateIndex("dbo.CompositionParam", "ParamID");
            AddForeignKey("dbo.CompositionParam", "ParamID", "dbo.Param", "ParamID");
            AddForeignKey("dbo.CompositionParam", "FlowFlowPropertyID", "dbo.FlowFlowProperty", "FlowFlowPropertyID");
        }
    }
}
