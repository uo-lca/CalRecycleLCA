CREATE TABLE [dbo].[CategorySystem] (
    [CategorySystemID]         INT           IDENTITY (1, 1) NOT NULL,
    [CategorySystem]           VARCHAR (100) NULL,
    [URI]                      VARCHAR (255) NULL,
    [CategorySystemDataTypeID] INT           NULL,
    [Delimeter]                VARCHAR (4)   NULL,
    [DataType-SQL]             VARCHAR (100) NULL,
    CONSTRAINT [PK_CategoryList_1] PRIMARY KEY CLUSTERED ([CategorySystemID] ASC)
);

