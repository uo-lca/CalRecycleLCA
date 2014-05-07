CREATE TABLE [dbo].[FragmentEmission] (
    [FragmentEmissionID] INT        IDENTITY (1, 1) NOT NULL,
    [NodeID]             INT        NULL,
    [FlowID]             INT        NULL,
    [DirectionID]        INT        NULL,
    [ScenarioID]         INT        NULL,
    [ParamID]            INT        NULL,
    [Quantity]           FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentEmission] PRIMARY KEY CLUSTERED ([FragmentEmissionID] ASC)
);



