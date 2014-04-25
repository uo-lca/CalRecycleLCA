CREATE TABLE [dbo].[Class] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [ClassID]              VARCHAR (60)  NULL,
    [ClassName]            VARCHAR (250) NULL,
    [CategorySystemID-SQL] INT           NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([ID] ASC)
);

