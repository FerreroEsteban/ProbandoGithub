CREATE TABLE [dbo].[UserBets] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [Token]      VARCHAR (36)   NOT NULL,
    [SportBetID] INT            NOT NULL,
    [Amount]     DECIMAL (8, 2) NOT NULL,
    [MatchCode]  VARCHAR (10)   NOT NULL,
    [Hit]        BIT            NOT NULL,
    [BetType]    VARCHAR (50)   NOT NULL,
    [BetPrice]   DECIMAL (8, 2) NOT NULL,
    [LinkedCode] VARCHAR (36)   NULL,
    CONSTRAINT [PK_UserBets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UserBets_FK] FOREIGN KEY ([SportBetID]) REFERENCES [dbo].[SportBets] ([ID])
);







