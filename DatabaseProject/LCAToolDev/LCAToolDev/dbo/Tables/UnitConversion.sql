CREATE TABLE [dbo].[UnitConversion] (
    [UnitConversionID]   INT          IDENTITY (1, 1) NOT NULL,
    [UnitConversionUUID] VARCHAR (36) NULL,
    [Unit]               VARCHAR (30) NULL,
    [UnitGroupID]        INT          NULL,
    [Conversion]         FLOAT (53)   NULL,
    [Ind-sql]            INT          NULL,
    [CreatedOn]          DATETIME     CONSTRAINT [UnitConversion_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]          INT          CONSTRAINT [UnitConversion_CreatedBy] DEFAULT ((0)) NOT NULL,
    [UpdatedOn]          DATETIME     CONSTRAINT [UnitConversion_UpdatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]          INT          CONSTRAINT [UnitConversion_UpdatedBy] DEFAULT ((0)) NOT NULL,
    [Voided]             BIT          CONSTRAINT [UnitConversion_Voided] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UnitConversion] PRIMARY KEY CLUSTERED ([UnitConversionID] ASC),
    CONSTRAINT [FK_UnitConversion_UnitGroup] FOREIGN KEY ([UnitGroupID]) REFERENCES [dbo].[UnitGroup] ([UnitGroupID])
);



