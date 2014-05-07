CREATE TABLE [dbo].[Class] (
    [ClassID]              INT           IDENTITY (1, 1) NOT NULL,
    [ExternalClassID]      VARCHAR (60)  NULL,
    [Name]                 VARCHAR (250) NULL,
    [CategorySystemID-SQL] INT           NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([ClassID] ASC)
);



