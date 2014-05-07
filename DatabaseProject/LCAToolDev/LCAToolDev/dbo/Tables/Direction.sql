CREATE TABLE [dbo].[Direction] (
    [DirectionID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NULL,
    CONSTRAINT [PK_Direction] PRIMARY KEY CLUSTERED ([DirectionID] ASC)
);



