CREATE TABLE [dbo].[FragmentScore] (
    [FragmentScoreID]     INT        IDENTITY (1, 1) NOT NULL,
    [FragmentID]          INT        NULL,
    [LCIAMethodID]        INT        NULL,
    [FragmentNodeStageID] INT        NULL,
    [ScenarioID]          INT        NULL,
    [ParamID]             INT        NULL,
    [ImpactScore]         FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentScore] PRIMARY KEY CLUSTERED ([FragmentScoreID] ASC)
);



