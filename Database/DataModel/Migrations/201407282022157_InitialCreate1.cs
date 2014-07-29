namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FragmentFlow", "ReferenceFlowPropertyID", "dbo.FlowProperty");
            DropIndex("dbo.FragmentFlow", new[] { "ReferenceFlowPropertyID" });
            AddColumn("dbo.FragmentNodeFragment", "FlowID", c => c.Int());
            AddColumn("dbo.NodeCache", "FlowMagnitude", c => c.Double());
            AddColumn("dbo.FragmentNodeProcess", "FlowID", c => c.Int());
            CreateIndex("dbo.FragmentNodeFragment", "FlowID");
            CreateIndex("dbo.FragmentNodeProcess", "FlowID");
            AddForeignKey("dbo.FragmentNodeFragment", "FlowID", "dbo.Flow", "FlowID");
            AddForeignKey("dbo.FragmentNodeProcess", "FlowID", "dbo.Flow", "FlowID");
            DropColumn("dbo.FragmentFlow", "ReferenceFlowPropertyID");
            DropColumn("dbo.FragmentFlow", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FragmentFlow", "Quantity", c => c.Double());
            AddColumn("dbo.FragmentFlow", "ReferenceFlowPropertyID", c => c.Int());
            DropForeignKey("dbo.FragmentNodeProcess", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.FragmentNodeFragment", "FlowID", "dbo.Flow");
            DropIndex("dbo.FragmentNodeProcess", new[] { "FlowID" });
            DropIndex("dbo.FragmentNodeFragment", new[] { "FlowID" });
            DropColumn("dbo.FragmentNodeProcess", "FlowID");
            DropColumn("dbo.NodeCache", "FlowMagnitude");
            DropColumn("dbo.FragmentNodeFragment", "FlowID");
            CreateIndex("dbo.FragmentFlow", "ReferenceFlowPropertyID");
            AddForeignKey("dbo.FragmentFlow", "ReferenceFlowPropertyID", "dbo.FlowProperty", "FlowPropertyID");
        }
    }
}
