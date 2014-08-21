namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyBackground : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Background", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.Background", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.Background", "NodeTypeID", "dbo.NodeType");
            DropIndex("dbo.Background", new[] { "FlowID" });
            DropIndex("dbo.Background", new[] { "DirectionID" });
            DropIndex("dbo.Background", new[] { "NodeTypeID" });
            AddColumn("dbo.Background", "ILCDEntityID", c => c.Int());
            AlterColumn("dbo.Background", "FlowID", c => c.Int(nullable: false));
            AlterColumn("dbo.Background", "DirectionID", c => c.Int(nullable: false));
            AlterColumn("dbo.Background", "NodeTypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.Param", "Name", c => c.String(maxLength: 255, unicode: false));
            CreateIndex("dbo.Background", "FlowID");
            CreateIndex("dbo.Background", "DirectionID");
            CreateIndex("dbo.Background", "NodeTypeID");
            CreateIndex("dbo.Background", "ILCDEntityID");
            AddForeignKey("dbo.Background", "ILCDEntityID", "dbo.ILCDEntity", "ILCDEntityID");
            AddForeignKey("dbo.Background", "DirectionID", "dbo.Direction", "DirectionID", cascadeDelete: true);
            AddForeignKey("dbo.Background", "FlowID", "dbo.Flow", "FlowID", cascadeDelete: true);
            AddForeignKey("dbo.Background", "NodeTypeID", "dbo.NodeType", "NodeTypeID", cascadeDelete: true);
            DropColumn("dbo.Background", "TargetID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Background", "TargetID", c => c.Int());
            DropForeignKey("dbo.Background", "NodeTypeID", "dbo.NodeType");
            DropForeignKey("dbo.Background", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.Background", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.Background", "ILCDEntityID", "dbo.ILCDEntity");
            DropIndex("dbo.Background", new[] { "ILCDEntityID" });
            DropIndex("dbo.Background", new[] { "NodeTypeID" });
            DropIndex("dbo.Background", new[] { "DirectionID" });
            DropIndex("dbo.Background", new[] { "FlowID" });
            AlterColumn("dbo.Param", "Name", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("dbo.Background", "NodeTypeID", c => c.Int());
            AlterColumn("dbo.Background", "DirectionID", c => c.Int());
            AlterColumn("dbo.Background", "FlowID", c => c.Int());
            DropColumn("dbo.Background", "ILCDEntityID");
            CreateIndex("dbo.Background", "NodeTypeID");
            CreateIndex("dbo.Background", "DirectionID");
            CreateIndex("dbo.Background", "FlowID");
            AddForeignKey("dbo.Background", "NodeTypeID", "dbo.NodeType", "NodeTypeID");
            AddForeignKey("dbo.Background", "FlowID", "dbo.Flow", "FlowID");
            AddForeignKey("dbo.Background", "DirectionID", "dbo.Direction", "DirectionID");
        }
    }
}
