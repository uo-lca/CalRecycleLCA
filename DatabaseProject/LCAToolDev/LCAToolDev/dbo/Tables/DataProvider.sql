CREATE TABLE [dbo].[DataProvider] (
    [DataProviderID]   INT           IDENTITY (1, 1) NOT NULL,
    [DataProviderUUID] VARCHAR (36)  NULL,
    [DataProviderName] VARCHAR (100) NULL,
    CONSTRAINT [PK_DataProvider] PRIMARY KEY CLUSTERED ([DataProviderID] ASC)
);

