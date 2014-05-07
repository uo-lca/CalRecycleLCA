CREATE TABLE [dbo].[IndicatorType] (
    [IndicatorTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (250) NULL,
    CONSTRAINT [PK_IndicatorType] PRIMARY KEY CLUSTERED ([IndicatorTypeID] ASC)
);

