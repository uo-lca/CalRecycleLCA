CREATE TABLE [dbo].[LCIAMethod] (
    [LCIAMethodID]             INT           IDENTITY (1, 1) NOT NULL,
    [LCIAMethodUUID]           VARCHAR (36)  NULL,
    [LCIAMethodVersion]        VARCHAR (15)  NULL,
    [LCIAMethod]               VARCHAR (255) NULL,
    [Methodology]              VARCHAR (60)  NULL,
    [ImpactCategory]           VARCHAR (255) NULL,
    [ImpactIndicator]          VARCHAR (MAX) NULL,
    [ReferenceYear]            VARCHAR (60)  NULL,
    [Duration]                 VARCHAR (255) NULL,
    [ImpactLocation]           VARCHAR (60)  NULL,
    [IndicatorType]            VARCHAR (60)  NULL,
    [Normalization]            BIT           NULL,
    [Weighting]                BIT           NULL,
    [UseAdvice]                VARCHAR (MAX) NULL,
    [LCIAMethodSourceID]       INT           NULL,
    [LCIAMethodFlowPropertyID] INT           NULL,
    [Source]                   VARCHAR (255) NULL,
    [ReferenceQuantity]        VARCHAR (255) NULL,
    CONSTRAINT [PK_LCIAMethod] PRIMARY KEY CLUSTERED ([LCIAMethodID] ASC),
    CONSTRAINT [FK_LCIAMethod_FlowProperty] FOREIGN KEY ([LCIAMethodFlowPropertyID]) REFERENCES [dbo].[FlowProperty] ([FlowPropertyID]),
    CONSTRAINT [FK_LCIAMethod_Source] FOREIGN KEY ([LCIAMethodSourceID]) REFERENCES [dbo].[Source] ([SourceID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_LCIAMethodUUID_LCIAMethodVersion]
    ON [dbo].[LCIAMethod]([LCIAMethodUUID] ASC, [LCIAMethodVersion] ASC);

