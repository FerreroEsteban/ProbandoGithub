CREATE TABLE [dbo].[Users] (
    [ID]           INT             IDENTITY (1, 1) NOT NULL,
    [UID]          VARCHAR (50)    NOT NULL,
    [NickName]     VARCHAR (100)   NOT NULL,
    [SessionToken] VARCHAR (255)   NOT NULL,
    [LaunchToken]  VARCHAR (255)   NOT NULL,
    [Balance]      DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
);

