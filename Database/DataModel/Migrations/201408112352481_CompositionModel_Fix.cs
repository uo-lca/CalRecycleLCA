namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompositionModel_Fix : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CompostionModel", newName: "CompositionModel");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.CompositionModel", newName: "CompostionModel");
        }
    }
}
