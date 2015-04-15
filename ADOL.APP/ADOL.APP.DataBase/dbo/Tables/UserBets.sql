CREATE TABLE [dbo].[UserBets] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [TransactionID] VARCHAR (36)   NOT NULL,
    [UserID]        INT            NOT NULL,
    [SportBetID]    INT            NOT NULL,
    [Amount]        DECIMAL (8, 2) NOT NULL,
    [MatchCode]     VARCHAR (10)   NOT NULL,
    [MatchName]     NVARCHAR (255) NOT NULL,
    [Hit]           BIT            NULL,
    [BetType]       VARCHAR (50)   NOT NULL,
    [BetPrice]      DECIMAL (8, 2) NOT NULL,
    [LinkedCode]    VARCHAR (36)   NULL,
    [PaymentStatus] INT            NULL,
    CONSTRAINT [PK_UserBets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UserBets_FK] FOREIGN KEY ([SportBetID]) REFERENCES [dbo].[SportBets] ([ID]),
    CONSTRAINT [UserBets_FK2] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);



