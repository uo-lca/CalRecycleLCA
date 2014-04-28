CREATE TABLE [dbo].[Classification] (
    [ClassificationID]               INT           IDENTITY (1, 1) NOT NULL,
    [ClassificationUUID]             VARCHAR (36)  NULL,
    [ClassificationCategorySystemID] INT           NULL,
    [ClassificationClassID]          INT           NULL,
    [ClassID-SQL]                    VARCHAR (100) NULL,
    [CategorySystem-SQL]             VARCHAR (200) NULL,
    CONSTRAINT [PK_Classifications_1] PRIMARY KEY CLUSTERED ([ClassificationID] ASC),
    CONSTRAINT [FK_Classification_CategorySystem] FOREIGN KEY ([ClassificationCategorySystemID]) REFERENCES [dbo].[CategorySystem] ([CategorySystemID]),
    CONSTRAINT [FK_Classification_Class] FOREIGN KEY ([ClassificationClassID]) REFERENCES [dbo].[Class] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ClassificationUUID_ClassificationCategorySystemID]
    ON [dbo].[Classification]([ClassificationUUID] ASC, [ClassificationCategorySystemID] ASC);

