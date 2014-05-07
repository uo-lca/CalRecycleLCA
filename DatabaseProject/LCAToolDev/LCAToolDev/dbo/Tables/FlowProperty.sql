CREATE TABLE [dbo].[FlowProperty] (
    [FlowPropertyID]      INT           IDENTITY (1, 1) NOT NULL,
    [FlowPropertyUUID]    CHAR (36)     NULL,
    [FlowPropertyVersion] VARCHAR (15)  NULL,
    [Name]                VARCHAR (255) NULL,
    [UnitGroupID]         INT           NULL,
    [UnitGroup_SQL]       VARCHAR (36)  NULL,
    CONSTRAINT [PK_FlowProperty] PRIMARY KEY CLUSTERED ([FlowPropertyID] ASC),
    CONSTRAINT [FK_FlowProperty_UnitGroup] FOREIGN KEY ([UnitGroupID]) REFERENCES [dbo].[UnitGroup] ([UnitGroupID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FlowPropertyUUID_FlowPropertyVersion]
    ON [dbo].[FlowProperty]([FlowPropertyUUID] ASC, [FlowPropertyVersion] ASC);

