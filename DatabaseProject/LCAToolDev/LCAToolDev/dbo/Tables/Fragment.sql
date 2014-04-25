﻿CREATE TABLE [dbo].[Fragment] (
    [FragmentID]   INT           IDENTITY (1, 1) NOT NULL,
    [FragmentName] VARCHAR (255) NULL,
    [Background]   BIT           NULL,
    CONSTRAINT [PK_Fragment] PRIMARY KEY CLUSTERED ([FragmentID] ASC)
);

