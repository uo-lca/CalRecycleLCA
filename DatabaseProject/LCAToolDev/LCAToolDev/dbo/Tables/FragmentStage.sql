CREATE TABLE [dbo].[FragmentStage] (
    [FragmentStageID] INT           IDENTITY (1, 1) NOT NULL,
    [FragmentID]      INT           NULL,
    [StageName]       VARCHAR (255) NULL,
    CONSTRAINT [PK_FragmentStage] PRIMARY KEY CLUSTERED ([FragmentStageID] ASC)
);

