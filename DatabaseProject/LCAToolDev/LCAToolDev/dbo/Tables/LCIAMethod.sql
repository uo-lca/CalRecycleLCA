CREATE TABLE [dbo].[LCIAMethod] (
    [LCIAMethodID]      INT           IDENTITY (1, 1) NOT NULL,
    [LCIAMethodUUID]    VARCHAR (36)  NULL,
    [LCIAMethodVersion] VARCHAR (15)  NULL,
    [Name]              VARCHAR (255) NULL,
    [Methodology]       VARCHAR (60)  NULL,
    [ImpactCategoryID]  INT           NULL,
    [ImpactIndicator]   VARCHAR (MAX) NULL,
    [ReferenceYear]     VARCHAR (60)  NULL,
    [Duration]          VARCHAR (255) NULL,
    [ImpactLocation]    VARCHAR (60)  NULL,
    [IndicatorTypeID]   INT           NULL,
    [Normalization]     BIT           NULL,
    [Weighting]         BIT           NULL,
    [UseAdvice]         VARCHAR (MAX) NULL,
    [SourceID]          INT           NULL,
    [FlowPropertyID]    INT           NULL,
    [Source]            VARCHAR (255) NULL,
    [ReferenceQuantity] VARCHAR (255) NULL,
    CONSTRAINT [PK_LCIAMethod] PRIMARY KEY CLUSTERED ([LCIAMethodID] ASC),
    CONSTRAINT [FK_LCIAMethod_FlowProperty] FOREIGN KEY ([FlowPropertyID]) REFERENCES [dbo].[FlowProperty] ([FlowPropertyID]),
    CONSTRAINT [FK_LCIAMethod_ImpactCategory] FOREIGN KEY ([ImpactCategoryID]) REFERENCES [dbo].[ImpactCategory] ([ImpactCategoryID]),
    CONSTRAINT [FK_LCIAMethod_IndicatorType] FOREIGN KEY ([IndicatorTypeID]) REFERENCES [dbo].[IndicatorType] ([IndicatorTypeID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_LCIAMethodUUID_LCIAMethodVersion]
    ON [dbo].[LCIAMethod]([LCIAMethodUUID] ASC, [LCIAMethodVersion] ASC);

