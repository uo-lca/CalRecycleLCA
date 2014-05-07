CREATE TABLE [dbo].[FragmentEdge] (
    [FragmentEdgeID] INT        IDENTITY (1, 1) NOT NULL,
    [FragmentID]     INT        NULL,
    [FragmentNodeID] INT        NULL,
    [FlowID]         INT        NULL,
    [DirectionID]    INT        NULL,
    [Terminus]       INT        NULL,
    [ScenarioID]     INT        NULL,
    [ParamID]        INT        NULL,
    [Quantity]       FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentEdge] PRIMARY KEY CLUSTERED ([FragmentEdgeID] ASC)
);



