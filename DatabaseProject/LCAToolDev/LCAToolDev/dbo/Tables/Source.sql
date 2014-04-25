CREATE TABLE [dbo].[Source] (
    [SourceID]      INT           IDENTITY (1, 1) NOT NULL,
    [SourceUUID]    VARCHAR (36)  NULL,
    [SourceVersion] VARCHAR (15)  NULL,
    [Source]        VARCHAR (255) NULL,
    [Citation]      VARCHAR (60)  NULL,
    [PubType]       VARCHAR (60)  NULL,
    [URI]           VARCHAR (255) NULL,
    CONSTRAINT [PK_Source_1] PRIMARY KEY CLUSTERED ([SourceID] ASC)
);

