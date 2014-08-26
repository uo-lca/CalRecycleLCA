namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorScenarioParam : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScenarioParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.ScenarioParam", "ScenarioID", "dbo.Scenario");
            DropIndex("dbo.ScenarioParam", new[] { "ScenarioID" });
            DropIndex("dbo.ScenarioParam", new[] { "ParamID" });
            AddColumn("dbo.Param", "ScenarioID", c => c.Int(nullable: false));
            CreateIndex("dbo.Param", "ScenarioID");
            AddForeignKey("dbo.Param", "ScenarioID", "dbo.Scenario", "ScenarioID", cascadeDelete: true);
            DropTable("dbo.ScenarioParam");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ScenarioParam",
                c => new
                    {
                        ScenarioParamID = c.Int(nullable: false),
                        ScenarioID = c.Int(),
                        ParamID = c.Int(),
                    })
                .PrimaryKey(t => t.ScenarioParamID);
            
            DropForeignKey("dbo.Param", "ScenarioID", "dbo.Scenario");
            DropIndex("dbo.Param", new[] { "ScenarioID" });
            DropColumn("dbo.Param", "ScenarioID");
            CreateIndex("dbo.ScenarioParam", "ParamID");
            CreateIndex("dbo.ScenarioParam", "ScenarioID");
            AddForeignKey("dbo.ScenarioParam", "ScenarioID", "dbo.Scenario", "ScenarioID");
            AddForeignKey("dbo.ScenarioParam", "ParamID", "dbo.Param", "ParamID");
        }
    }
}
