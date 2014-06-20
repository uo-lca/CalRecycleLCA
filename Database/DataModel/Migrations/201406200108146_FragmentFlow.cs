namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FragmentFlow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DependencyParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.NodeEmissionParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropPrimaryKey("dbo.FragmentFlow");
            AlterColumn("dbo.FragmentFlow", "FragmentFlowID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.FragmentFlow", "FragmentFlowID");
            AddForeignKey("dbo.DependencyParam", "FragmentFlowID", "dbo.FragmentFlow", "FragmentFlowID");
            AddForeignKey("dbo.NodeEmissionParam", "FragmentFlowID", "dbo.FragmentFlow", "FragmentFlowID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NodeEmissionParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.DependencyParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropPrimaryKey("dbo.FragmentFlow");
            AlterColumn("dbo.FragmentFlow", "FragmentFlowID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.FragmentFlow", "FragmentFlowID");
            AddForeignKey("dbo.NodeEmissionParam", "FragmentFlowID", "dbo.FragmentFlow", "FragmentFlowID");
            AddForeignKey("dbo.DependencyParam", "FragmentFlowID", "dbo.FragmentFlow", "FragmentFlowID");
        }
    }
}
