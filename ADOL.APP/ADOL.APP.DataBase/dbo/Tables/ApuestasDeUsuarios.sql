CREATE TABLE [dbo].[ApuestasDeUsuarios] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [Token]              VARCHAR (36)   NOT NULL,
    [Amount]             DECIMAL (8, 2) NOT NULL,
    [ApuestaDeportivaID] INT            NOT NULL,
    [Acierto]            BIT            NULL,
    [BetType]            VARCHAR (50)   NOT NULL,
    [BetPrice]           DECIMAL (8, 2) NOT NULL,
    [Linked]             VARCHAR (36)   NULL,
    CONSTRAINT [PK_ApuestasDeUsuarios] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [ApuestasDeUsuarios_FK1] FOREIGN KEY ([ApuestaDeportivaID]) REFERENCES [dbo].[ApuestasDeportivas] ([ID])
);





