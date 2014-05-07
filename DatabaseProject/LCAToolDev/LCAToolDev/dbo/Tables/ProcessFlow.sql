CREATE TABLE [dbo].[ProcessFlow] (
    [ProcessFlowID] INT          IDENTITY (1, 1) NOT NULL,
    [ProcessUUID]   VARCHAR (36) NULL,
    [ProcessID]     INT          NULL,
    [FlowID]        INT          NULL,
    [DirectionID]   INT          NULL,
    [Type]          VARCHAR (15) NULL,
    [VarName]       VARCHAR (15) NULL,
    [Magnitude]     FLOAT (53)   NULL,
    [Result]        FLOAT (53)   NULL,
    [STDev]         FLOAT (53)   NULL,
    [Flow-SQL]      VARCHAR (50) NULL,
    [Direction-SQL] VARCHAR (50) NULL,
    [Ind-SQL]       INT          NULL,
    [Geography]     VARCHAR (15) NULL,
    CONSTRAINT [PK_ProcessFlow_1] PRIMARY KEY CLUSTERED ([ProcessFlowID] ASC),
    CONSTRAINT [FK_ProcessFlow_Direction] FOREIGN KEY ([DirectionID]) REFERENCES [dbo].[Direction] ([DirectionID]),
    CONSTRAINT [FK_ProcessFlow_Flow] FOREIGN KEY ([FlowID]) REFERENCES [dbo].[Flow] ([FlowID]),
    CONSTRAINT [FK_ProcessFlow_Process] FOREIGN KEY ([ProcessID]) REFERENCES [dbo].[Process] ([ProcessID])
);



