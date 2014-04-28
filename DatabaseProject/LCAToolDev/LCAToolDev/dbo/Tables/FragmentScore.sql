CREATE TABLE [dbo].[FragmentScore] (
    [FragmentScoreID]                  INT        IDENTITY (1, 1) NOT NULL,
    [FragmentScoreFragmentID]          INT        NULL,
    [FragmentScoreLCIAMethodID]        INT        NULL,
    [FragmentScoreFragmentNodeStageID] INT        NULL,
    [FragmentScoreScenarioID]          INT        NULL,
    [FragmentScoreParamID]             INT        NULL,
    [ImpactScore]                      FLOAT (53) NULL,
    CONSTRAINT [PK_FragmentScore] PRIMARY KEY CLUSTERED ([FragmentScoreID] ASC)
);

