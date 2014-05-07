CREATE TABLE [dbo].[Flow] (
    [FlowID]                    INT           IDENTITY (1, 1) NOT NULL,
    [FlowUUID]                  CHAR (36)     NULL,
    [FlowVersion]               VARCHAR (15)  NULL,
    [Name]                      VARCHAR (255) NULL,
    [CASNumber]                 CHAR (15)     NULL,
    [FlowPropertyID]            INT           NULL,
    [FlowTypeID]                INT           NULL,
    [FlowType_SQL]              VARCHAR (200) NULL,
    [ReferenceFlowProperty_SQL] VARCHAR (36)  NULL,
    CONSTRAINT [PK_ElementaryFlow_1] PRIMARY KEY CLUSTERED ([FlowID] ASC),
    CONSTRAINT [FK_Flow_FlowProperty] FOREIGN KEY ([FlowPropertyID]) REFERENCES [dbo].[FlowProperty] ([FlowPropertyID]),
    CONSTRAINT [FK_Flow_FlowType] FOREIGN KEY ([FlowTypeID]) REFERENCES [dbo].[FlowType] ([FlowTypeID])
);



