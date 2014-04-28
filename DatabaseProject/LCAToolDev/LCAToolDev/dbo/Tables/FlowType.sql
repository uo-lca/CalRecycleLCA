CREATE TABLE [dbo].[FlowType] (
    [FlowTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [FlowType]   VARCHAR (100) NULL,
    CONSTRAINT [PK_FlowType] PRIMARY KEY CLUSTERED ([FlowTypeID] ASC)
);

