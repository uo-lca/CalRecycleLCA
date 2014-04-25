CREATE TABLE [dbo].[Flow] (
    [FlowID]                    INT           IDENTITY (1, 1) NOT NULL,
    [FlowUUID]                  CHAR (36)     NULL,
    [FlowVersion]               VARCHAR (15)  NULL,
    [Flow]                      VARCHAR (255) NULL,
    [CASNumber]                 CHAR (15)     NULL,
    [FlowPropertyID]            INT           NULL,
    [FlowTypeID]                INT           NULL,
    [FlowMass]                  REAL          NULL,
    [FlowMassChg]               INT           NULL,
    [FlowType-SQL]              VARCHAR (200) NULL,
    [ReferenceFlowProperty-SQL] VARCHAR (36)  NULL,
    CONSTRAINT [PK_ElementaryFlow_1] PRIMARY KEY CLUSTERED ([FlowID] ASC),
    CONSTRAINT [FK_Flow_FlowProperty] FOREIGN KEY ([FlowPropertyID]) REFERENCES [dbo].[FlowProperty] ([FlowPropertyID]),
    CONSTRAINT [FK_Flow_FlowType] FOREIGN KEY ([FlowTypeID]) REFERENCES [dbo].[FlowType] ([FlowTypeID])
);

