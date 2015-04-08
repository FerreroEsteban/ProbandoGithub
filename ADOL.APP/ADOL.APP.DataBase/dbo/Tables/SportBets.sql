CREATE TABLE [dbo].[SportBets] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Code]         VARCHAR (10)   NOT NULL,
    [Name]         NCHAR (10)     NOT NULL,
    [Active]       BIT            NOT NULL,
    [Odd1]         DECIMAL (8, 2) NOT NULL,
    [Odd2]         DECIMAL (8, 2) NULL,
    [Odd3]         DECIMAL (8, 2) NULL,
    [Odd4]         DECIMAL (8, 2) NULL,
    [LastUpdate]   DATETIME       NOT NULL,
    [SportEventID] INT            NOT NULL,
    CONSTRAINT [PK_SportBets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [SportBets_FK1] FOREIGN KEY ([SportEventID]) REFERENCES [dbo].[SportEvents] ([ID])
);

