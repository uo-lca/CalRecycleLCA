CREATE TABLE [dbo].[ImpactCategory] (
    [ImpactCategoryID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (250) NULL,
    CONSTRAINT [PK_ImpactCategory] PRIMARY KEY CLUSTERED ([ImpactCategoryID] ASC)
);

