namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessReferenceYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Process", "ReferenceYear", c => c.String(maxLength: 60, unicode: false));
            DropColumn("dbo.Process", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Process", "Year", c => c.String(maxLength: 60, unicode: false));
            DropColumn("dbo.Process", "ReferenceYear");
        }
    }
}
