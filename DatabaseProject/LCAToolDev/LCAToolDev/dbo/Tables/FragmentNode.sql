CREATE TABLE [dbo].[FragmentNode] (
    [FragmentNodeID]         INT          IDENTITY (0, 1) NOT NULL,
    [FragmentNodeFragmentID] INT          NULL,
    [FragmentNodeProcessID]  INT          NULL,
    [FragmentNodeName]       VARCHAR (30) NULL,
    [FragmentNodeStageID]    INT          NULL,
    [FragmentNodeScenarioID] INT          NULL,
    [FragmentNodeParamID]    INT          NULL,
    [FragmentNodeWeight]     FLOAT (53)   CONSTRAINT [DF_FragmentNode_FragmentNodeWeight] DEFAULT ((1)) NULL
);

