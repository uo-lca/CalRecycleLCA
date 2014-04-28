CREATE TABLE [dbo].[Category] (
    [CategoryID]                         INT           IDENTITY (1, 1) NOT NULL,
    [CategorySystemID]                   INT           NULL,
    [CategoryClassID]                    INT           NULL,
    [ParentClassID-notneededremovelater] INT           NULL,
    [DataTypeID-notneededremovelater]    INT           NULL,
    [HierarchyLevel]                     INT           NULL,
    [Hier]                               VARCHAR (250) NULL,
    [ClassID-SQL]                        VARCHAR (60)  NULL,
    [Parent-SQL]                         VARCHAR (60)  NULL,
    [ClassName-SQL]                      VARCHAR (100) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryID] ASC),
    CONSTRAINT [FK_Category_CategorySystem] FOREIGN KEY ([CategorySystemID]) REFERENCES [dbo].[CategorySystem] ([CategorySystemID]),
    CONSTRAINT [FK_Category_Class] FOREIGN KEY ([CategoryClassID]) REFERENCES [dbo].[Class] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories]
    ON [dbo].[Category]([CategoryID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CategorySystemID_CategoryClassID]
    ON [dbo].[Category]([CategorySystemID] ASC, [CategoryClassID] ASC);

