CREATE TABLE [dbo].[ProcessFlow] (
    [ProcessFlowID]          INT          IDENTITY (1, 1) NOT NULL,
    [ProcessUUID]            VARCHAR (36) NULL,
    [ProcessFlowProcessID]   INT          NULL,
    [ProcessFlowFlowID]      INT          NULL,
    [ProcessFlowDirectionID] INT          NULL,
    [ProcessFlowType]        VARCHAR (15) NULL,
    [VarName]                VARCHAR (15) NULL,
    [Magnitude]              FLOAT (53)   NULL,
    [Result]                 FLOAT (53)   NULL,
    [STDev]                  FLOAT (53)   NULL,
    [Flow-SQL]               VARCHAR (50) NULL,
    [Direction-SQL]          VARCHAR (50) NULL,
    [Ind-SQL]                INT          NULL,
    [Geography]              VARCHAR (15) NULL,
    CONSTRAINT [PK_ProcessFlow_1] PRIMARY KEY CLUSTERED ([ProcessFlowID] ASC),
    CONSTRAINT [FK_ProcessFlow_Direction] FOREIGN KEY ([ProcessFlowDirectionID]) REFERENCES [dbo].[Direction] ([DirectionID]),
    CONSTRAINT [FK_ProcessFlow_Flow] FOREIGN KEY ([ProcessFlowFlowID]) REFERENCES [dbo].[Flow] ([FlowID]),
    CONSTRAINT [FK_ProcessFlow_Process] FOREIGN KEY ([ProcessFlowProcessID]) REFERENCES [dbo].[Process] ([ProcessID])
);

