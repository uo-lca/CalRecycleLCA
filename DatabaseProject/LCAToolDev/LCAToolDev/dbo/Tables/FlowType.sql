CREATE TABLE [dbo].[FlowType] (
    [FlowTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Type]       VARCHAR (100) NULL,
    CONSTRAINT [PK_FlowType] PRIMARY KEY CLUSTERED ([FlowTypeID] ASC)
);



