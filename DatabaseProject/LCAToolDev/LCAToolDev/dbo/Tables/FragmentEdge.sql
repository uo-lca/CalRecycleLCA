CREATE TABLE [dbo].[FragmentEdge] (
    [FragmentEdgeID]             INT        IDENTITY (1, 1) NOT NULL,
    [FragmentEdgeFragmentID]     INT        NULL,
    [FragmentEdgeFragmentNodeID] INT        NULL,
    [FragmentEdgeFlowID]         INT        NULL,
    [FragmentEdgeDirectionID]    INT        NULL,
    [Terminus]                   INT        NULL,
    [FragmentEdgeScenarioID]     INT        NULL,
    [FragmentEdgeParamID]        INT        NULL,
    [Quantity]                   FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentEdge] PRIMARY KEY CLUSTERED ([FragmentEdgeID] ASC)
);

