CREATE TABLE [dbo].[FlowFlowProperty] (
    [FlowPropertyVersionID]   INT          IDENTITY (1, 1) NOT NULL,
    [FlowPropertyVersionUUID] VARCHAR (36) NULL,
    [FlowID]                  INT          NULL,
    [FlowPropertyID]          INT          NULL,
    [MeanValue]               FLOAT (53)   NULL,
    [StDev]                   FLOAT (53)   NULL,
    [FlowProperty-SQL]        VARCHAR (36) NULL,
    [FlowReference-SQL]       VARCHAR (36) NULL,
    [Ind-SQL]                 INT          NULL,
    CONSTRAINT [PK_FlowPropertyVersion] PRIMARY KEY CLUSTERED ([FlowPropertyVersionID] ASC),
    CONSTRAINT [FK_FlowPropertyVersion_Flow] FOREIGN KEY ([FlowID]) REFERENCES [dbo].[Flow] ([FlowID])
);

