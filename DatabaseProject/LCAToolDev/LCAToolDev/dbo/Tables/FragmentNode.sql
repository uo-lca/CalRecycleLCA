CREATE TABLE [dbo].[FragmentNode] (
    [FragmentNodeID] INT          IDENTITY (0, 1) NOT NULL,
    [FragmentID]     INT          NULL,
    [ProcessID]      INT          NULL,
    [Name]           VARCHAR (30) NULL,
    [StageID]        INT          NULL,
    [ScenarioID]     INT          NULL,
    [ParamID]        INT          NULL,
    [Weight]         FLOAT (53)   CONSTRAINT [DF_FragmentNode_FragmentNodeWeight] DEFAULT ((1)) NULL
);



