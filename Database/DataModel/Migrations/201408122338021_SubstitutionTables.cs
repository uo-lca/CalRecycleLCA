namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubstitutionTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FragmentSubstitution",
                c => new
                    {
                        FragmentNodeFragmentID = c.Int(nullable: false),
                        ScenarioID = c.Int(nullable: false),
                        SubFragmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FragmentNodeFragmentID, t.ScenarioID })
                .ForeignKey("dbo.FragmentNodeFragment", t => t.FragmentNodeFragmentID, cascadeDelete: true)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID, cascadeDelete: true)
                .ForeignKey("dbo.Fragment", t => t.SubFragmentID, cascadeDelete: true)
                .Index(t => t.FragmentNodeFragmentID)
                .Index(t => t.ScenarioID)
                .Index(t => t.SubFragmentID);
            
            CreateTable(
                "dbo.ProcessSubstitution",
                c => new
                    {
                        FragmentNodeProcessID = c.Int(nullable: false),
                        ScenarioID = c.Int(nullable: false),
                        ProcessID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FragmentNodeProcessID, t.ScenarioID })
                .ForeignKey("dbo.Process", t => t.ProcessID, cascadeDelete: true)
                .ForeignKey("dbo.FragmentNodeProcess", t => t.FragmentNodeProcessID, cascadeDelete: true)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID, cascadeDelete: true)
                .Index(t => t.FragmentNodeProcessID)
                .Index(t => t.ScenarioID)
                .Index(t => t.ProcessID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FragmentSubstitution", "SubFragmentID", "dbo.Fragment");
            DropForeignKey("dbo.ProcessSubstitution", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.ProcessSubstitution", "FragmentNodeProcessID", "dbo.FragmentNodeProcess");
            DropForeignKey("dbo.ProcessSubstitution", "ProcessID", "dbo.Process");
            DropForeignKey("dbo.FragmentSubstitution", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.FragmentSubstitution", "FragmentNodeFragmentID", "dbo.FragmentNodeFragment");
            DropIndex("dbo.ProcessSubstitution", new[] { "ProcessID" });
            DropIndex("dbo.ProcessSubstitution", new[] { "ScenarioID" });
            DropIndex("dbo.ProcessSubstitution", new[] { "FragmentNodeProcessID" });
            DropIndex("dbo.FragmentSubstitution", new[] { "SubFragmentID" });
            DropIndex("dbo.FragmentSubstitution", new[] { "ScenarioID" });
            DropIndex("dbo.FragmentSubstitution", new[] { "FragmentNodeFragmentID" });
            DropTable("dbo.ProcessSubstitution");
            DropTable("dbo.FragmentSubstitution");
        }
    }
}
