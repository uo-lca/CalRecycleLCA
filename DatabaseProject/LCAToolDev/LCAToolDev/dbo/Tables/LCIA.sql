CREATE TABLE [dbo].[LCIA] (
    [LCIAID]          INT           IDENTITY (1, 1) NOT NULL,
    [LCIAMethodID]    INT           NULL,
    [LCIAUUID]        CHAR (36)     NULL,
    [LCIAFlowID]      INT           NULL,
    [LCIADirectionID] INT           NULL,
    [Factor]          FLOAT (53)    NULL,
    [Flow-SQL]        VARCHAR (36)  NULL,
    [Direction-SQL]   VARCHAR (100) NULL,
    CONSTRAINT [PK_LCIA] PRIMARY KEY CLUSTERED ([LCIAID] ASC),
    CONSTRAINT [FK_LCIA_Direction] FOREIGN KEY ([LCIADirectionID]) REFERENCES [dbo].[Direction] ([DirectionID]),
    CONSTRAINT [FK_LCIA_LCIAMethod] FOREIGN KEY ([LCIAMethodID]) REFERENCES [dbo].[LCIAMethod] ([LCIAMethodID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LCIA]
    ON [dbo].[LCIA]([LCIAID] ASC);

