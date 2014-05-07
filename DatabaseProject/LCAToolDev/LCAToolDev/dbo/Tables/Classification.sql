CREATE TABLE [dbo].[Classification] (
    [ClassificationID]   INT           IDENTITY (1, 1) NOT NULL,
    [ClassificationUUID] VARCHAR (36)  NULL,
    [CategorySystemID]   INT           NULL,
    [ClassID]            INT           NULL,
    [ClassID-SQL]        VARCHAR (100) NULL,
    [CategorySystem-SQL] VARCHAR (200) NULL,
    CONSTRAINT [PK_Classifications_1] PRIMARY KEY CLUSTERED ([ClassificationID] ASC),
    CONSTRAINT [FK_Classification_CategorySystem] FOREIGN KEY ([CategorySystemID]) REFERENCES [dbo].[CategorySystem] ([CategorySystemID]),
    CONSTRAINT [FK_Classification_Class] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Class] ([ClassID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ClassificationUUID_ClassificationCategorySystemID]
    ON [dbo].[Classification]([ClassificationUUID] ASC, [CategorySystemID] ASC);



