CREATE TABLE [dbo].[FragmentEmission] (
    [FragmentEmissionID]          INT        IDENTITY (1, 1) NOT NULL,
    [FragmentEmissionNodeID]      INT        NULL,
    [FragmentEmissionFlowID]      INT        NULL,
    [FragmentEmissionDirectionID] INT        NULL,
    [FragmentEmissionScenarioID]  INT        NULL,
    [FragmentEmissionParamID]     INT        NULL,
    [Quantity]                    FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentEmission] PRIMARY KEY CLUSTERED ([FragmentEmissionID] ASC)
);

