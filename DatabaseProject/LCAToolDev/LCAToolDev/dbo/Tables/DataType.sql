CREATE TABLE [dbo].[DataType] (
    [DataTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (250) NULL,
    CONSTRAINT [PK_DataType] PRIMARY KEY CLUSTERED ([DataTypeID] ASC)
);



