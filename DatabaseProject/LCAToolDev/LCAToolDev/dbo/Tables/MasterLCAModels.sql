CREATE TABLE [dbo].[MasterLCAModels] (
    [UUID]       VARCHAR (36)  NULL,
    [Version]    VARCHAR (15)  NULL,
    [DataTypeID] INT           NULL,
    [URI]        VARCHAR (255) NULL,
    [OldIndex]   VARCHAR (255) NULL,
    CONSTRAINT [FK_MasterLCAModels_DataType] FOREIGN KEY ([DataTypeID]) REFERENCES [dbo].[DataType] ([DataTypeID])
);

