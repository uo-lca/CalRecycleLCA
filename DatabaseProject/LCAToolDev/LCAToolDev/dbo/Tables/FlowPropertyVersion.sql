CREATE TABLE [dbo].[FlowPropertyVersion] (
    [FlowPropertyVersionID]             INT          IDENTITY (1, 1) NOT NULL,
    [FlowPropertyVersionUUID]           VARCHAR (36) NULL,
    [FlowPropertyVersionFlowID]         INT          NULL,
    [FlowPropertyVersionFlowPropertyID] INT          NULL,
    [MeanValue]                         FLOAT (53)   NULL,
    [StDev]                             FLOAT (53)   NULL,
    [FlowProperty-SQL]                  VARCHAR (36) NULL,
    [FlowReference-SQL]                 VARCHAR (36) NULL,
    [Ind-SQL]                           INT          NULL,
    CONSTRAINT [PK_FlowPropertyVersion] PRIMARY KEY CLUSTERED ([FlowPropertyVersionID] ASC),
    CONSTRAINT [FK_FlowPropertyVersion_Flow] FOREIGN KEY ([FlowPropertyVersionFlowID]) REFERENCES [dbo].[Flow] ([FlowID]),
    CONSTRAINT [FK_FlowPropertyVersion_FlowProperty] FOREIGN KEY ([FlowPropertyVersionFlowPropertyID]) REFERENCES [dbo].[FlowProperty] ([FlowPropertyID])
);

