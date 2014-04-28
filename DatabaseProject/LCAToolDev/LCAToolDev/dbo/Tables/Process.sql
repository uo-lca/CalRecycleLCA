CREATE TABLE [dbo].[Process] (
    [ProcessID]         INT           IDENTITY (1, 1) NOT NULL,
    [ProcessUUID]       VARCHAR (36)  NULL,
    [ProcessVersion]    VARCHAR (255) NULL,
    [Process]           VARCHAR (255) NULL,
    [Year]              VARCHAR (60)  NULL,
    [Geography]         VARCHAR (15)  NULL,
    [ReferenceFlow-SQL] VARCHAR (36)  NULL,
    [RefererenceType]   VARCHAR (60)  NULL,
    [ProcessType]       VARCHAR (60)  NULL,
    [Diagram]           VARCHAR (60)  NULL,
    [ProcessFlowID]     INT           NULL,
    CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED ([ProcessID] ASC),
    CONSTRAINT [FK_Process_Flow] FOREIGN KEY ([ProcessFlowID]) REFERENCES [dbo].[Flow] ([FlowID])
);

