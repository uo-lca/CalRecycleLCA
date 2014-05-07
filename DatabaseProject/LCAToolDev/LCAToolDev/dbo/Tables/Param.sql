CREATE TABLE [dbo].[Param] (
    [ParamID]       INT          IDENTITY (1, 1) NOT NULL,
    [ScenarioSetID] INT          NULL,
    [Name]          VARCHAR (30) NULL,
    [FlowID]        INT          NULL,
    [ParamDefault]  FLOAT (53)   CONSTRAINT [DF_Param_ParamDefault] DEFAULT ((1)) NULL,
    [Min]           FLOAT (53)   NULL,
    [Max]           FLOAT (53)   NULL,
    [Scale]         FLOAT (53)   NULL,
    CONSTRAINT [PK_Param] PRIMARY KEY CLUSTERED ([ParamID] ASC)
);



