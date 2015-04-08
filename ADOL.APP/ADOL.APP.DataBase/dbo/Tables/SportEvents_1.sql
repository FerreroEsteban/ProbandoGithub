CREATE TABLE [dbo].[SportEvents] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Code]        VARCHAR (50)   NOT NULL,
    [Active]      BIT            NOT NULL,
    [Init]        DATETIME       NOT NULL,
    [End]         DATETIME       NOT NULL,
    [Home]        NVARCHAR (50)  NOT NULL,
    [Away]        NVARCHAR (50)  NOT NULL,
    [SportID]     INT            NOT NULL,
    [LeagueCode]  VARCHAR (50)   NOT NULL,
    [CountryCode] VARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_SportEvents] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [SportEvents_FK1] FOREIGN KEY ([SportID]) REFERENCES [dbo].[Sports] ([ID])
);

