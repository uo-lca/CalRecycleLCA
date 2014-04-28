CREATE TABLE [dbo].[UnitConversion] (
    [UnitConversionID]          INT          IDENTITY (1, 1) NOT NULL,
    [UnitConversionUUID]        VARCHAR (36) NULL,
    [Unit]                      VARCHAR (30) NULL,
    [UnitConversionUnitGroupID] INT          NULL,
    [Conversion]                FLOAT (53)   NULL,
    [Ind-sql]                   INT          NULL,
    CONSTRAINT [PK_UnitConversion] PRIMARY KEY CLUSTERED ([UnitConversionID] ASC)
);

