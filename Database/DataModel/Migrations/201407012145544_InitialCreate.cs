namespace LcaDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BackgroundCache",
                c => new
                    {
                        BackgroundCacheID = c.Int(nullable: false),
                        BackgroundID = c.Int(),
                        ScenarioID = c.Int(),
                        LCIAMethodID = c.Int(),
                        Score = c.Double(),
                    })
                .PrimaryKey(t => t.BackgroundCacheID)
                .ForeignKey("dbo.Background", t => t.BackgroundID)
                .ForeignKey("dbo.LCIAMethod", t => t.LCIAMethodID)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID)
                .Index(t => t.BackgroundID)
                .Index(t => t.ScenarioID)
                .Index(t => t.LCIAMethodID);
            
            CreateTable(
                "dbo.Background",
                c => new
                    {
                        BackgroundID = c.Int(nullable: false),
                        FlowID = c.Int(),
                        DirectionID = c.Int(),
                        NodeTypeID = c.Int(),
                        TargetID = c.Int(),
                    })
                .PrimaryKey(t => t.BackgroundID)
                .ForeignKey("dbo.Direction", t => t.DirectionID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .ForeignKey("dbo.NodeType", t => t.NodeTypeID)
                .Index(t => t.FlowID)
                .Index(t => t.DirectionID)
                .Index(t => t.NodeTypeID);
            
            CreateTable(
                "dbo.Direction",
                c => new
                    {
                        DirectionID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.DirectionID);
            
            CreateTable(
                "dbo.FragmentFlow",
                c => new
                    {
                        FragmentFlowID = c.Int(nullable: false),
                        FragmentID = c.Int(),
                        Name = c.String(unicode: false),
                        FragmentStageID = c.Int(),
                        ReferenceFlowPropertyID = c.Int(),
                        NodeTypeID = c.Int(),
                        FlowID = c.Int(),
                        DirectionID = c.Int(),
                        Quantity = c.Double(),
                        ParentFragmentFlowID = c.Int(),
                    })
                .PrimaryKey(t => t.FragmentFlowID)
                .ForeignKey("dbo.FlowProperty", t => t.ReferenceFlowPropertyID)
                .ForeignKey("dbo.Fragment", t => t.FragmentID)
                .ForeignKey("dbo.FragmentStage", t => t.FragmentStageID)
                .ForeignKey("dbo.NodeType", t => t.NodeTypeID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .ForeignKey("dbo.Direction", t => t.DirectionID)
                .ForeignKey("dbo.FragmentFlow", t => t.ParentFragmentFlowID)
                .Index(t => t.FragmentID)
                .Index(t => t.FragmentStageID)
                .Index(t => t.ReferenceFlowPropertyID)
                .Index(t => t.NodeTypeID)
                .Index(t => t.FlowID)
                .Index(t => t.DirectionID)
                .Index(t => t.ParentFragmentFlowID);
            
            CreateTable(
                "dbo.DependencyParam",
                c => new
                    {
                        DependencyParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        FragmentFlowID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.DependencyParamID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .Index(t => t.ParamID)
                .Index(t => t.FragmentFlowID);
            
            CreateTable(
                "dbo.DistributionParam",
                c => new
                    {
                        DistributionParamID = c.Int(nullable: false, identity: true),
                        DependencyParamID = c.Int(),
                        ConservationParamID = c.Int(),
                    })
                .PrimaryKey(t => t.DistributionParamID)
                .ForeignKey("dbo.DependencyParam", t => t.ConservationParamID)
                .ForeignKey("dbo.DependencyParam", t => t.DependencyParamID)
                .Index(t => t.DependencyParamID)
                .Index(t => t.ConservationParamID);
            
            CreateTable(
                "dbo.Param",
                c => new
                    {
                        ParamID = c.Int(nullable: false, identity: true),
                        ParamTypeID = c.Int(),
                        Name = c.String(maxLength: 30, unicode: false),
                        Min = c.Double(),
                        Typ = c.Double(),
                        Max = c.Double(),
                    })
                .PrimaryKey(t => t.ParamID)
                .ForeignKey("dbo.ParamType", t => t.ParamTypeID)
                .Index(t => t.ParamTypeID);
            
            CreateTable(
                "dbo.CharacterizationParam",
                c => new
                    {
                        CharacterizationParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        LCAID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.CharacterizationParamID)
                .ForeignKey("dbo.LCIA", t => t.LCAID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .Index(t => t.ParamID)
                .Index(t => t.LCAID);
            
            CreateTable(
                "dbo.LCIA",
                c => new
                    {
                        LCIAID = c.Int(nullable: false, identity: true),
                        LCIAMethodID = c.Int(),
                        FlowID = c.Int(),
                        FlowUUID = c.String(maxLength: 36),
                        FlowName = c.String(maxLength: 255),
                        Geography = c.String(maxLength: 100, unicode: false),
                        DirectionID = c.Int(),
                        Factor = c.Double(),
                    })
                .PrimaryKey(t => t.LCIAID)
                .ForeignKey("dbo.Direction", t => t.DirectionID)
                .ForeignKey("dbo.LCIAMethod", t => t.LCIAMethodID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .Index(t => t.LCIAMethodID)
                .Index(t => t.FlowID)
                .Index(t => t.DirectionID);
            
            CreateTable(
                "dbo.Flow",
                c => new
                    {
                        FlowID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, unicode: false),
                        CASNumber = c.String(maxLength: 15, fixedLength: true, unicode: false),
                        ReferenceFlowProperty = c.Int(),
                        FlowTypeID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.FlowID)
                .ForeignKey("dbo.FlowProperty", t => t.ReferenceFlowProperty)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.FlowType", t => t.FlowTypeID)
                .Index(t => t.ReferenceFlowProperty)
                .Index(t => t.FlowTypeID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.CompostionModel",
                c => new
                    {
                        CompositionModelID = c.Int(nullable: false, identity: true),
                        FlowID = c.Int(),
                        Name = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.CompositionModelID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .Index(t => t.FlowID);
            
            CreateTable(
                "dbo.CompositionData",
                c => new
                    {
                        CompositionDataID = c.Int(nullable: false, identity: true),
                        CompositionModelID = c.Int(),
                        FlowFlowPropertyID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.CompositionDataID)
                .ForeignKey("dbo.CompostionModel", t => t.CompositionModelID)
                .ForeignKey("dbo.FlowFlowProperty", t => t.FlowFlowPropertyID)
                .Index(t => t.CompositionModelID)
                .Index(t => t.FlowFlowPropertyID);
            
            CreateTable(
                "dbo.FlowFlowProperty",
                c => new
                    {
                        FlowFlowPropertyID = c.Int(nullable: false, identity: true),
                        FlowID = c.Int(),
                        FlowPropertyID = c.Int(),
                        MeanValue = c.Double(),
                        StDev = c.Double(),
                    })
                .PrimaryKey(t => t.FlowFlowPropertyID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .Index(t => t.FlowID);
            
            CreateTable(
                "dbo.CompositionParam",
                c => new
                    {
                        CompositionParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        FlowFlowPropertyID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.CompositionParamID)
                .ForeignKey("dbo.FlowFlowProperty", t => t.FlowFlowPropertyID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .Index(t => t.ParamID)
                .Index(t => t.FlowFlowPropertyID);
            
            CreateTable(
                "dbo.FlowPropertyParam",
                c => new
                    {
                        FlowPropertyParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        FlowFlowPropertyID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.FlowPropertyParamID)
                .ForeignKey("dbo.FlowFlowProperty", t => t.FlowFlowPropertyID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .Index(t => t.ParamID)
                .Index(t => t.FlowFlowPropertyID);
            
            CreateTable(
                "dbo.FlowProperty",
                c => new
                    {
                        FlowPropertyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, unicode: false),
                        UnitGroupID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.FlowPropertyID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.UnitGroup", t => t.UnitGroupID)
                .Index(t => t.UnitGroupID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.FlowPropertyEmission",
                c => new
                    {
                        FlowPropertyEmissionID = c.Int(nullable: false, identity: true),
                        FlowPropertyID = c.Int(),
                        FlowID = c.Int(),
                        Scale = c.Double(),
                    })
                .PrimaryKey(t => t.FlowPropertyEmissionID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .ForeignKey("dbo.FlowProperty", t => t.FlowPropertyID)
                .Index(t => t.FlowPropertyID)
                .Index(t => t.FlowID);
            
            CreateTable(
                "dbo.ILCDEntity",
                c => new
                    {
                        ILCDEntityID = c.Int(nullable: false, identity: true),
                        UUID = c.String(nullable: false, maxLength: 36, unicode: false),
                        Version = c.String(maxLength: 15, unicode: false),
                        DataProviderID = c.Int(),
                        DataTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.ILCDEntityID)
                .ForeignKey("dbo.DataType", t => t.DataTypeID)
                .ForeignKey("dbo.DataProvider", t => t.DataProviderID)
                .Index(t => t.DataProviderID)
                .Index(t => t.DataTypeID);
            
            CreateTable(
                "dbo.Classification",
                c => new
                    {
                        ClassificationID = c.Int(nullable: false),
                        CategoryID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.ClassificationID)
                .ForeignKey("dbo.Category", t => t.CategoryID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .Index(t => t.CategoryID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false),
                        ExternalClassID = c.String(maxLength: 60, unicode: false),
                        Name = c.String(maxLength: 250, unicode: false),
                        CategorySystemID = c.Int(),
                        ParentCategoryID = c.Int(),
                        HierarchyLevel = c.Int(),
                    })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.Category", t => t.ParentCategoryID)
                .ForeignKey("dbo.CategorySystem", t => t.CategorySystemID)
                .Index(t => t.CategorySystemID)
                .Index(t => t.ParentCategoryID);
            
            CreateTable(
                "dbo.CategorySystem",
                c => new
                    {
                        CategorySystemID = c.Int(nullable: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        DataTypeID = c.Int(),
                        Delimiter = c.String(maxLength: 4, unicode: false),
                    })
                .PrimaryKey(t => t.CategorySystemID)
                .ForeignKey("dbo.DataType", t => t.DataTypeID)
                .Index(t => t.DataTypeID);
            
            CreateTable(
                "dbo.DataType",
                c => new
                    {
                        DataTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.DataTypeID);
            
            CreateTable(
                "dbo.DataProvider",
                c => new
                    {
                        DataProviderID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                        DirName = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.DataProviderID);
            
            CreateTable(
                "dbo.Fragment",
                c => new
                    {
                        FragmentID = c.Int(nullable: false),
                        Name = c.String(maxLength: 255, unicode: false),
                        ReferenceFragmentFlowID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.FragmentID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.FragmentFlow", t => t.ReferenceFragmentFlowID)
                .Index(t => t.ReferenceFragmentFlowID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.FragmentNodeFragment",
                c => new
                    {
                        FragmentNodeFragmentID = c.Int(nullable: false),
                        FragmentFlowID = c.Int(),
                        SubFragmentID = c.Int(),
                    })
                .PrimaryKey(t => t.FragmentNodeFragmentID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Fragment", t => t.SubFragmentID)
                .Index(t => t.FragmentFlowID)
                .Index(t => t.SubFragmentID);
            
            CreateTable(
                "dbo.FragmentStage",
                c => new
                    {
                        FragmentStageID = c.Int(nullable: false, identity: true),
                        FragmentID = c.Int(),
                        StageName = c.String(maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.FragmentStageID)
                .ForeignKey("dbo.Fragment", t => t.FragmentID)
                .Index(t => t.FragmentID);
            
            CreateTable(
                "dbo.LCIAMethod",
                c => new
                    {
                        LCIAMethodID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, unicode: false),
                        Methodology = c.String(maxLength: 60, unicode: false),
                        ImpactCategoryID = c.Int(),
                        ImpactIndicator = c.String(unicode: false),
                        ReferenceYear = c.String(maxLength: 60, unicode: false),
                        Duration = c.String(maxLength: 255, unicode: false),
                        ImpactLocation = c.String(maxLength: 60, unicode: false),
                        IndicatorTypeID = c.Int(),
                        Normalization = c.Boolean(),
                        Weighting = c.Boolean(),
                        UseAdvice = c.String(unicode: false),
                        ReferenceQuantity = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.LCIAMethodID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.ImpactCategory", t => t.ImpactCategoryID)
                .ForeignKey("dbo.IndicatorType", t => t.IndicatorTypeID)
                .ForeignKey("dbo.FlowProperty", t => t.ReferenceQuantity)
                .Index(t => t.ImpactCategoryID)
                .Index(t => t.IndicatorTypeID)
                .Index(t => t.ReferenceQuantity)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.ImpactCategory",
                c => new
                    {
                        ImpactCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.ImpactCategoryID);
            
            CreateTable(
                "dbo.IndicatorType",
                c => new
                    {
                        IndicatorTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.IndicatorTypeID);
            
            CreateTable(
                "dbo.ScoreCache",
                c => new
                    {
                        ScoreCacheID = c.Int(nullable: false),
                        NodeCacheID = c.Int(),
                        LCIAMethodID = c.Int(),
                        ImpactScore = c.Double(),
                    })
                .PrimaryKey(t => t.ScoreCacheID)
                .ForeignKey("dbo.LCIAMethod", t => t.LCIAMethodID)
                .ForeignKey("dbo.NodeCache", t => t.NodeCacheID)
                .Index(t => t.NodeCacheID)
                .Index(t => t.LCIAMethodID);
            
            CreateTable(
                "dbo.NodeCache",
                c => new
                    {
                        NodeCacheID = c.Int(nullable: false, identity: true),
                        FragmentFlowID = c.Int(),
                        ScenarioID = c.Int(),
                        NodeWeight = c.Double(),
                    })
                .PrimaryKey(t => t.NodeCacheID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID)
                .Index(t => t.FragmentFlowID)
                .Index(t => t.ScenarioID);
            
            CreateTable(
                "dbo.Scenario",
                c => new
                    {
                        ScenarioID = c.Int(nullable: false),
                        ScenarioGroupID = c.Int(),
                        Name = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.ScenarioID)
                .ForeignKey("dbo.ScenarioGroup", t => t.ScenarioGroupID)
                .Index(t => t.ScenarioGroupID);
            
            CreateTable(
                "dbo.ScenarioBackground",
                c => new
                    {
                        ScenarioBackgroundID = c.Int(nullable: false),
                        ScenarioID = c.Int(),
                        FlowID = c.Int(),
                        NodeTypeID = c.Int(),
                        TargetID = c.Int(),
                    })
                .PrimaryKey(t => t.ScenarioBackgroundID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .ForeignKey("dbo.NodeType", t => t.NodeTypeID)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID)
                .Index(t => t.ScenarioID)
                .Index(t => t.FlowID)
                .Index(t => t.NodeTypeID);
            
            CreateTable(
                "dbo.NodeType",
                c => new
                    {
                        NodeTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.NodeTypeID);
            
            CreateTable(
                "dbo.ScenarioGroup",
                c => new
                    {
                        ScenarioGroupID = c.Int(nullable: false),
                        OwnedBy = c.Int(),
                        Name = c.String(maxLength: 50, unicode: false),
                        VisibilityID = c.Int(),
                    })
                .PrimaryKey(t => t.ScenarioGroupID)
                .ForeignKey("dbo.User", t => t.OwnedBy)
                .ForeignKey("dbo.Visibility", t => t.VisibilityID)
                .Index(t => t.OwnedBy)
                .Index(t => t.VisibilityID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                        CanLogin = c.Boolean(),
                        CanEditScenarios = c.Boolean(),
                        CanEditFragments = c.Boolean(),
                        CanEditBackground = c.Boolean(),
                        CanAppend = c.Boolean(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Visibility",
                c => new
                    {
                        VisibilityID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.VisibilityID);
            
            CreateTable(
                "dbo.ScenarioParam",
                c => new
                    {
                        ScenarioParamID = c.Int(nullable: false),
                        ScenarioID = c.Int(),
                        ParamID = c.Int(),
                    })
                .PrimaryKey(t => t.ScenarioParamID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .ForeignKey("dbo.Scenario", t => t.ScenarioID)
                .Index(t => t.ScenarioID)
                .Index(t => t.ParamID);
            
            CreateTable(
                "dbo.Process",
                c => new
                    {
                        ProcessID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, unicode: false),
                        Year = c.String(maxLength: 60, unicode: false),
                        Geography = c.String(maxLength: 15, unicode: false),
                        ReferenceTypeID = c.Int(),
                        ProcessTypeID = c.Int(),
                        ReferenceFlowID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.ProcessID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.ProcessType", t => t.ProcessTypeID)
                .ForeignKey("dbo.ReferenceType", t => t.ReferenceTypeID)
                .ForeignKey("dbo.Flow", t => t.ReferenceFlowID)
                .Index(t => t.ReferenceTypeID)
                .Index(t => t.ProcessTypeID)
                .Index(t => t.ReferenceFlowID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.FragmentNodeProcess",
                c => new
                    {
                        FragmentNodeProcessID = c.Int(nullable: false),
                        FragmentFlowID = c.Int(),
                        ProcessID = c.Int(),
                    })
                .PrimaryKey(t => t.FragmentNodeProcessID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Process", t => t.ProcessID)
                .Index(t => t.FragmentFlowID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.ProcessFlow",
                c => new
                    {
                        ProcessFlowID = c.Int(nullable: false, identity: true),
                        ProcessID = c.Int(),
                        FlowID = c.Int(),
                        DirectionID = c.Int(),
                        Type = c.String(maxLength: 15, unicode: false),
                        VarName = c.String(maxLength: 15, unicode: false),
                        Magnitude = c.Double(),
                        Result = c.Double(),
                        STDev = c.Double(),
                        Geography = c.String(maxLength: 15, unicode: false),
                    })
                .PrimaryKey(t => t.ProcessFlowID)
                .ForeignKey("dbo.Direction", t => t.DirectionID)
                .ForeignKey("dbo.Flow", t => t.FlowID)
                .ForeignKey("dbo.Process", t => t.ProcessID)
                .Index(t => t.ProcessID)
                .Index(t => t.FlowID)
                .Index(t => t.DirectionID);
            
            CreateTable(
                "dbo.NodeEmissionParam",
                c => new
                    {
                        NodeEmissionParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        ProcessFlowID = c.Int(),
                        FragmentFlowID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.NodeEmissionParamID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .ForeignKey("dbo.ProcessFlow", t => t.ProcessFlowID)
                .Index(t => t.ParamID)
                .Index(t => t.ProcessFlowID)
                .Index(t => t.FragmentFlowID);
            
            CreateTable(
                "dbo.ProcessDissipation",
                c => new
                    {
                        ProcessDissipationID = c.Int(nullable: false, identity: true),
                        ProcessFlowID = c.Int(),
                        FlowPropertyEmissionID = c.Int(),
                        EmissionFactor = c.Double(),
                    })
                .PrimaryKey(t => t.ProcessDissipationID)
                .ForeignKey("dbo.ProcessFlow", t => t.ProcessFlowID)
                .Index(t => t.ProcessFlowID);
            
            CreateTable(
                "dbo.NodeDissipationParam",
                c => new
                    {
                        NodeDissipationParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        ProcessDissipationID = c.Int(),
                        FragmentFlowID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.NodeDissipationParamID)
                .ForeignKey("dbo.FragmentFlow", t => t.FragmentFlowID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .ForeignKey("dbo.ProcessDissipation", t => t.ProcessDissipationID)
                .Index(t => t.ParamID)
                .Index(t => t.ProcessDissipationID)
                .Index(t => t.FragmentFlowID);
            
            CreateTable(
                "dbo.ProcessDissipationParam",
                c => new
                    {
                        ProcessDissipationParamID = c.Int(nullable: false),
                        ParamID = c.Int(),
                        ProcessDissipationID = c.Int(),
                    })
                .PrimaryKey(t => t.ProcessDissipationParamID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .ForeignKey("dbo.ProcessDissipation", t => t.ProcessDissipationID)
                .Index(t => t.ParamID)
                .Index(t => t.ProcessDissipationID);
            
            CreateTable(
                "dbo.ProcessEmissionParam",
                c => new
                    {
                        ProcessEmissionParamID = c.Int(nullable: false, identity: true),
                        ParamID = c.Int(),
                        ProcessFlowID = c.Int(),
                        Value = c.Double(),
                    })
                .PrimaryKey(t => t.ProcessEmissionParamID)
                .ForeignKey("dbo.Param", t => t.ParamID)
                .ForeignKey("dbo.ProcessFlow", t => t.ProcessFlowID)
                .Index(t => t.ParamID)
                .Index(t => t.ProcessFlowID);
            
            CreateTable(
                "dbo.ProcessType",
                c => new
                    {
                        ProcessTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.ProcessTypeID);
            
            CreateTable(
                "dbo.ReferenceType",
                c => new
                    {
                        ReferenceTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.ReferenceTypeID);
            
            CreateTable(
                "dbo.UnitGroup",
                c => new
                    {
                        UnitGroupID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                        ReferenceUnitConversionID = c.Int(),
                        ILCDEntityID = c.Int(),
                    })
                .PrimaryKey(t => t.UnitGroupID)
                .ForeignKey("dbo.ILCDEntity", t => t.ILCDEntityID)
                .ForeignKey("dbo.UnitConversion", t => t.ReferenceUnitConversionID)
                .Index(t => t.ReferenceUnitConversionID)
                .Index(t => t.ILCDEntityID);
            
            CreateTable(
                "dbo.UnitConversion",
                c => new
                    {
                        UnitConversionID = c.Int(nullable: false, identity: true),
                        Unit = c.String(maxLength: 30, unicode: false),
                        UnitGroupID = c.Int(),
                        LongName = c.String(maxLength: 250, unicode: false),
                        Conversion = c.Double(),
                    })
                .PrimaryKey(t => t.UnitConversionID)
                .ForeignKey("dbo.UnitGroup", t => t.UnitGroupID)
                .Index(t => t.UnitGroupID);
            
            CreateTable(
                "dbo.FlowType",
                c => new
                    {
                        FlowTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.FlowTypeID);
            
            CreateTable(
                "dbo.ParamType",
                c => new
                    {
                        ParamTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                    })
                .PrimaryKey(t => t.ParamTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fragment", "ReferenceFragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.FragmentFlow", "ParentFragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.FragmentFlow", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.Param", "ParamTypeID", "dbo.ParamType");
            DropForeignKey("dbo.DependencyParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.CharacterizationParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.Process", "ReferenceFlowID", "dbo.Flow");
            DropForeignKey("dbo.LCIA", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.FragmentFlow", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.Flow", "FlowTypeID", "dbo.FlowType");
            DropForeignKey("dbo.LCIAMethod", "ReferenceQuantity", "dbo.FlowProperty");
            DropForeignKey("dbo.UnitConversion", "UnitGroupID", "dbo.UnitGroup");
            DropForeignKey("dbo.UnitGroup", "ReferenceUnitConversionID", "dbo.UnitConversion");
            DropForeignKey("dbo.UnitGroup", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.FlowProperty", "UnitGroupID", "dbo.UnitGroup");
            DropForeignKey("dbo.Process", "ReferenceTypeID", "dbo.ReferenceType");
            DropForeignKey("dbo.Process", "ProcessTypeID", "dbo.ProcessType");
            DropForeignKey("dbo.ProcessEmissionParam", "ProcessFlowID", "dbo.ProcessFlow");
            DropForeignKey("dbo.ProcessEmissionParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.ProcessDissipation", "ProcessFlowID", "dbo.ProcessFlow");
            DropForeignKey("dbo.ProcessDissipationParam", "ProcessDissipationID", "dbo.ProcessDissipation");
            DropForeignKey("dbo.ProcessDissipationParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.NodeDissipationParam", "ProcessDissipationID", "dbo.ProcessDissipation");
            DropForeignKey("dbo.NodeDissipationParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.NodeDissipationParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.ProcessFlow", "ProcessID", "dbo.Process");
            DropForeignKey("dbo.NodeEmissionParam", "ProcessFlowID", "dbo.ProcessFlow");
            DropForeignKey("dbo.NodeEmissionParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.NodeEmissionParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.ProcessFlow", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.ProcessFlow", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.Process", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.FragmentNodeProcess", "ProcessID", "dbo.Process");
            DropForeignKey("dbo.FragmentNodeProcess", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.ScoreCache", "NodeCacheID", "dbo.NodeCache");
            DropForeignKey("dbo.ScenarioParam", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.ScenarioParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.ScenarioGroup", "VisibilityID", "dbo.Visibility");
            DropForeignKey("dbo.ScenarioGroup", "OwnedBy", "dbo.User");
            DropForeignKey("dbo.Scenario", "ScenarioGroupID", "dbo.ScenarioGroup");
            DropForeignKey("dbo.ScenarioBackground", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.ScenarioBackground", "NodeTypeID", "dbo.NodeType");
            DropForeignKey("dbo.FragmentFlow", "NodeTypeID", "dbo.NodeType");
            DropForeignKey("dbo.Background", "NodeTypeID", "dbo.NodeType");
            DropForeignKey("dbo.ScenarioBackground", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.NodeCache", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.BackgroundCache", "ScenarioID", "dbo.Scenario");
            DropForeignKey("dbo.NodeCache", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.ScoreCache", "LCIAMethodID", "dbo.LCIAMethod");
            DropForeignKey("dbo.LCIA", "LCIAMethodID", "dbo.LCIAMethod");
            DropForeignKey("dbo.LCIAMethod", "IndicatorTypeID", "dbo.IndicatorType");
            DropForeignKey("dbo.LCIAMethod", "ImpactCategoryID", "dbo.ImpactCategory");
            DropForeignKey("dbo.LCIAMethod", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.BackgroundCache", "LCIAMethodID", "dbo.LCIAMethod");
            DropForeignKey("dbo.Fragment", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.FragmentFlow", "FragmentStageID", "dbo.FragmentStage");
            DropForeignKey("dbo.FragmentStage", "FragmentID", "dbo.Fragment");
            DropForeignKey("dbo.FragmentNodeFragment", "SubFragmentID", "dbo.Fragment");
            DropForeignKey("dbo.FragmentNodeFragment", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.FragmentFlow", "FragmentID", "dbo.Fragment");
            DropForeignKey("dbo.Flow", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.FlowProperty", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.ILCDEntity", "DataProviderID", "dbo.DataProvider");
            DropForeignKey("dbo.Classification", "ILCDEntityID", "dbo.ILCDEntity");
            DropForeignKey("dbo.Classification", "CategoryID", "dbo.Category");
            DropForeignKey("dbo.ILCDEntity", "DataTypeID", "dbo.DataType");
            DropForeignKey("dbo.CategorySystem", "DataTypeID", "dbo.DataType");
            DropForeignKey("dbo.Category", "CategorySystemID", "dbo.CategorySystem");
            DropForeignKey("dbo.Category", "ParentCategoryID", "dbo.Category");
            DropForeignKey("dbo.FragmentFlow", "ReferenceFlowPropertyID", "dbo.FlowProperty");
            DropForeignKey("dbo.Flow", "ReferenceFlowProperty", "dbo.FlowProperty");
            DropForeignKey("dbo.FlowPropertyEmission", "FlowPropertyID", "dbo.FlowProperty");
            DropForeignKey("dbo.FlowPropertyEmission", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.CompostionModel", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.FlowPropertyParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.FlowPropertyParam", "FlowFlowPropertyID", "dbo.FlowFlowProperty");
            DropForeignKey("dbo.FlowFlowProperty", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.CompositionParam", "ParamID", "dbo.Param");
            DropForeignKey("dbo.CompositionParam", "FlowFlowPropertyID", "dbo.FlowFlowProperty");
            DropForeignKey("dbo.CompositionData", "FlowFlowPropertyID", "dbo.FlowFlowProperty");
            DropForeignKey("dbo.CompositionData", "CompositionModelID", "dbo.CompostionModel");
            DropForeignKey("dbo.Background", "FlowID", "dbo.Flow");
            DropForeignKey("dbo.LCIA", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.CharacterizationParam", "LCAID", "dbo.LCIA");
            DropForeignKey("dbo.DependencyParam", "FragmentFlowID", "dbo.FragmentFlow");
            DropForeignKey("dbo.DistributionParam", "DependencyParamID", "dbo.DependencyParam");
            DropForeignKey("dbo.DistributionParam", "ConservationParamID", "dbo.DependencyParam");
            DropForeignKey("dbo.Background", "DirectionID", "dbo.Direction");
            DropForeignKey("dbo.BackgroundCache", "BackgroundID", "dbo.Background");
            DropIndex("dbo.UnitConversion", new[] { "UnitGroupID" });
            DropIndex("dbo.UnitGroup", new[] { "ILCDEntityID" });
            DropIndex("dbo.UnitGroup", new[] { "ReferenceUnitConversionID" });
            DropIndex("dbo.ProcessEmissionParam", new[] { "ProcessFlowID" });
            DropIndex("dbo.ProcessEmissionParam", new[] { "ParamID" });
            DropIndex("dbo.ProcessDissipationParam", new[] { "ProcessDissipationID" });
            DropIndex("dbo.ProcessDissipationParam", new[] { "ParamID" });
            DropIndex("dbo.NodeDissipationParam", new[] { "FragmentFlowID" });
            DropIndex("dbo.NodeDissipationParam", new[] { "ProcessDissipationID" });
            DropIndex("dbo.NodeDissipationParam", new[] { "ParamID" });
            DropIndex("dbo.ProcessDissipation", new[] { "ProcessFlowID" });
            DropIndex("dbo.NodeEmissionParam", new[] { "FragmentFlowID" });
            DropIndex("dbo.NodeEmissionParam", new[] { "ProcessFlowID" });
            DropIndex("dbo.NodeEmissionParam", new[] { "ParamID" });
            DropIndex("dbo.ProcessFlow", new[] { "DirectionID" });
            DropIndex("dbo.ProcessFlow", new[] { "FlowID" });
            DropIndex("dbo.ProcessFlow", new[] { "ProcessID" });
            DropIndex("dbo.FragmentNodeProcess", new[] { "ProcessID" });
            DropIndex("dbo.FragmentNodeProcess", new[] { "FragmentFlowID" });
            DropIndex("dbo.Process", new[] { "ILCDEntityID" });
            DropIndex("dbo.Process", new[] { "ReferenceFlowID" });
            DropIndex("dbo.Process", new[] { "ProcessTypeID" });
            DropIndex("dbo.Process", new[] { "ReferenceTypeID" });
            DropIndex("dbo.ScenarioParam", new[] { "ParamID" });
            DropIndex("dbo.ScenarioParam", new[] { "ScenarioID" });
            DropIndex("dbo.ScenarioGroup", new[] { "VisibilityID" });
            DropIndex("dbo.ScenarioGroup", new[] { "OwnedBy" });
            DropIndex("dbo.ScenarioBackground", new[] { "NodeTypeID" });
            DropIndex("dbo.ScenarioBackground", new[] { "FlowID" });
            DropIndex("dbo.ScenarioBackground", new[] { "ScenarioID" });
            DropIndex("dbo.Scenario", new[] { "ScenarioGroupID" });
            DropIndex("dbo.NodeCache", new[] { "ScenarioID" });
            DropIndex("dbo.NodeCache", new[] { "FragmentFlowID" });
            DropIndex("dbo.ScoreCache", new[] { "LCIAMethodID" });
            DropIndex("dbo.ScoreCache", new[] { "NodeCacheID" });
            DropIndex("dbo.LCIAMethod", new[] { "ILCDEntityID" });
            DropIndex("dbo.LCIAMethod", new[] { "ReferenceQuantity" });
            DropIndex("dbo.LCIAMethod", new[] { "IndicatorTypeID" });
            DropIndex("dbo.LCIAMethod", new[] { "ImpactCategoryID" });
            DropIndex("dbo.FragmentStage", new[] { "FragmentID" });
            DropIndex("dbo.FragmentNodeFragment", new[] { "SubFragmentID" });
            DropIndex("dbo.FragmentNodeFragment", new[] { "FragmentFlowID" });
            DropIndex("dbo.Fragment", new[] { "ILCDEntityID" });
            DropIndex("dbo.Fragment", new[] { "ReferenceFragmentFlowID" });
            DropIndex("dbo.CategorySystem", new[] { "DataTypeID" });
            DropIndex("dbo.Category", new[] { "ParentCategoryID" });
            DropIndex("dbo.Category", new[] { "CategorySystemID" });
            DropIndex("dbo.Classification", new[] { "ILCDEntityID" });
            DropIndex("dbo.Classification", new[] { "CategoryID" });
            DropIndex("dbo.ILCDEntity", new[] { "DataTypeID" });
            DropIndex("dbo.ILCDEntity", new[] { "DataProviderID" });
            DropIndex("dbo.FlowPropertyEmission", new[] { "FlowID" });
            DropIndex("dbo.FlowPropertyEmission", new[] { "FlowPropertyID" });
            DropIndex("dbo.FlowProperty", new[] { "ILCDEntityID" });
            DropIndex("dbo.FlowProperty", new[] { "UnitGroupID" });
            DropIndex("dbo.FlowPropertyParam", new[] { "FlowFlowPropertyID" });
            DropIndex("dbo.FlowPropertyParam", new[] { "ParamID" });
            DropIndex("dbo.CompositionParam", new[] { "FlowFlowPropertyID" });
            DropIndex("dbo.CompositionParam", new[] { "ParamID" });
            DropIndex("dbo.FlowFlowProperty", new[] { "FlowID" });
            DropIndex("dbo.CompositionData", new[] { "FlowFlowPropertyID" });
            DropIndex("dbo.CompositionData", new[] { "CompositionModelID" });
            DropIndex("dbo.CompostionModel", new[] { "FlowID" });
            DropIndex("dbo.Flow", new[] { "ILCDEntityID" });
            DropIndex("dbo.Flow", new[] { "FlowTypeID" });
            DropIndex("dbo.Flow", new[] { "ReferenceFlowProperty" });
            DropIndex("dbo.LCIA", new[] { "DirectionID" });
            DropIndex("dbo.LCIA", new[] { "FlowID" });
            DropIndex("dbo.LCIA", new[] { "LCIAMethodID" });
            DropIndex("dbo.CharacterizationParam", new[] { "LCAID" });
            DropIndex("dbo.CharacterizationParam", new[] { "ParamID" });
            DropIndex("dbo.Param", new[] { "ParamTypeID" });
            DropIndex("dbo.DistributionParam", new[] { "ConservationParamID" });
            DropIndex("dbo.DistributionParam", new[] { "DependencyParamID" });
            DropIndex("dbo.DependencyParam", new[] { "FragmentFlowID" });
            DropIndex("dbo.DependencyParam", new[] { "ParamID" });
            DropIndex("dbo.FragmentFlow", new[] { "ParentFragmentFlowID" });
            DropIndex("dbo.FragmentFlow", new[] { "DirectionID" });
            DropIndex("dbo.FragmentFlow", new[] { "FlowID" });
            DropIndex("dbo.FragmentFlow", new[] { "NodeTypeID" });
            DropIndex("dbo.FragmentFlow", new[] { "ReferenceFlowPropertyID" });
            DropIndex("dbo.FragmentFlow", new[] { "FragmentStageID" });
            DropIndex("dbo.FragmentFlow", new[] { "FragmentID" });
            DropIndex("dbo.Background", new[] { "NodeTypeID" });
            DropIndex("dbo.Background", new[] { "DirectionID" });
            DropIndex("dbo.Background", new[] { "FlowID" });
            DropIndex("dbo.BackgroundCache", new[] { "LCIAMethodID" });
            DropIndex("dbo.BackgroundCache", new[] { "ScenarioID" });
            DropIndex("dbo.BackgroundCache", new[] { "BackgroundID" });
            DropTable("dbo.ParamType");
            DropTable("dbo.FlowType");
            DropTable("dbo.UnitConversion");
            DropTable("dbo.UnitGroup");
            DropTable("dbo.ReferenceType");
            DropTable("dbo.ProcessType");
            DropTable("dbo.ProcessEmissionParam");
            DropTable("dbo.ProcessDissipationParam");
            DropTable("dbo.NodeDissipationParam");
            DropTable("dbo.ProcessDissipation");
            DropTable("dbo.NodeEmissionParam");
            DropTable("dbo.ProcessFlow");
            DropTable("dbo.FragmentNodeProcess");
            DropTable("dbo.Process");
            DropTable("dbo.ScenarioParam");
            DropTable("dbo.Visibility");
            DropTable("dbo.User");
            DropTable("dbo.ScenarioGroup");
            DropTable("dbo.NodeType");
            DropTable("dbo.ScenarioBackground");
            DropTable("dbo.Scenario");
            DropTable("dbo.NodeCache");
            DropTable("dbo.ScoreCache");
            DropTable("dbo.IndicatorType");
            DropTable("dbo.ImpactCategory");
            DropTable("dbo.LCIAMethod");
            DropTable("dbo.FragmentStage");
            DropTable("dbo.FragmentNodeFragment");
            DropTable("dbo.Fragment");
            DropTable("dbo.DataProvider");
            DropTable("dbo.DataType");
            DropTable("dbo.CategorySystem");
            DropTable("dbo.Category");
            DropTable("dbo.Classification");
            DropTable("dbo.ILCDEntity");
            DropTable("dbo.FlowPropertyEmission");
            DropTable("dbo.FlowProperty");
            DropTable("dbo.FlowPropertyParam");
            DropTable("dbo.CompositionParam");
            DropTable("dbo.FlowFlowProperty");
            DropTable("dbo.CompositionData");
            DropTable("dbo.CompostionModel");
            DropTable("dbo.Flow");
            DropTable("dbo.LCIA");
            DropTable("dbo.CharacterizationParam");
            DropTable("dbo.Param");
            DropTable("dbo.DistributionParam");
            DropTable("dbo.DependencyParam");
            DropTable("dbo.FragmentFlow");
            DropTable("dbo.Direction");
            DropTable("dbo.Background");
            DropTable("dbo.BackgroundCache");
        }
    }
}
