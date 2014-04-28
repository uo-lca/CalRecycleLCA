CREATE TABLE [dbo].[FlowProperty] (
    [FlowPropertyID]          INT           IDENTITY (1, 1) NOT NULL,
    [FlowPropertyUUID]        CHAR (36)     NULL,
    [FlowPropertyVersion]     VARCHAR (15)  NULL,
    [FlowProperty]            VARCHAR (255) NULL,
    [FlowPropertyUnitGroupID] INT           NULL,
    [UnitGroup-SQL]           VARCHAR (36)  NULL,
    CONSTRAINT [PK_FlowProperty] PRIMARY KEY CLUSTERED ([FlowPropertyID] ASC),
    CONSTRAINT [FK_FlowProperty_UnitGroup] FOREIGN KEY ([FlowPropertyUnitGroupID]) REFERENCES [dbo].[UnitGroup] ([UnitGroupID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FlowPropertyUUID_FlowPropertyVersion]
    ON [dbo].[FlowProperty]([FlowPropertyUUID] ASC, [FlowPropertyVersion] ASC);

