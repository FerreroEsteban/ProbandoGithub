CREATE TABLE [dbo].[ApuestasDeUsuarios] (
    [ID]                 INT          IDENTITY (1, 1) NOT NULL,
    [Token]              VARCHAR (32) NOT NULL,
    [Amount]             DECIMAL (18) NOT NULL,
    [ApuestaDeportivaID] INT          NOT NULL,
    [EventoDeportivoID]  INT          NOT NULL,
    [Acierto]            BIT          NOT NULL,
    CONSTRAINT [PK_ApuestasDeUsuarios] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [ApuestasDeUsuarios_FK1] FOREIGN KEY ([ApuestaDeportivaID]) REFERENCES [dbo].[ApuestasDeportivas] ([ID]),
    CONSTRAINT [ApuestasDeUsuarios_FK2] FOREIGN KEY ([EventoDeportivoID]) REFERENCES [dbo].[EventosDeportivos] ([ID])
);

