CREATE TABLE [dbo].[ScenarioSet] (
    [ScenarioSetID]   INT          IDENTITY (1, 1) NOT NULL,
    [ScenarioSetName] VARCHAR (30) NULL,
    CONSTRAINT [PK_ScenarioSet] PRIMARY KEY CLUSTERED ([ScenarioSetID] ASC)
);

