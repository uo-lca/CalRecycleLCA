CREATE TABLE [dbo].[UnitGroup] (
    [UnitGroupID]   INT           IDENTITY (1, 1) NOT NULL,
    [UnitGroupUUID] CHAR (36)     NOT NULL,
    [Version]       VARCHAR (15)  NULL,
    [UnitGroup]     VARCHAR (100) NULL,
    [ReferenceUnit] VARCHAR (100) NULL,
    CONSTRAINT [PK_UnitGroup] PRIMARY KEY CLUSTERED ([UnitGroupID] ASC)
);

