CREATE TABLE [dbo].[UnitGroup] (
    [UnitGroupID]   INT           IDENTITY (1, 1) NOT NULL,
    [UnitGroupUUID] CHAR (36)     NOT NULL,
    [Version]       VARCHAR (15)  NULL,
    [Name]          VARCHAR (100) NULL,
    [ReferenceUnit] VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [CONSTRAINT_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     INT           CONSTRAINT [CONSTRAINT_CreatedBy] DEFAULT ((0)) NOT NULL,
    [UpdatedOn]     DATETIME      CONSTRAINT [CONSTRAINT_UpdatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]     INT           CONSTRAINT [CONSTRAINT_UpdatedBy] DEFAULT ((0)) NOT NULL,
    [Voided]        BIT           CONSTRAINT [CONSTRAINT_Voided] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UnitGroup] PRIMARY KEY CLUSTERED ([UnitGroupID] ASC)
);



