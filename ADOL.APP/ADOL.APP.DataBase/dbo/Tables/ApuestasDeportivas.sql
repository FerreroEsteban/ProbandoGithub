CREATE TABLE [dbo].[ApuestasDeportivas] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]            VARCHAR (10)   NOT NULL,
    [Nombre]            NCHAR (10)     NOT NULL,
    [Activo]            BIT            NOT NULL,
    [Odd1]              DECIMAL (8, 2) NOT NULL,
    [Odd2]              DECIMAL (8, 2) NULL,
    [Odd3]              DECIMAL (8, 2) NULL,
    [Odd4]              DECIMAL (8, 2) NULL,
    [Acualizado]        DATETIME       NOT NULL,
    [EventoDeportivoID] INT            NOT NULL,
    CONSTRAINT [PK_ApuestasDeportivas] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [ApuestasDeportivas_FK1] FOREIGN KEY ([EventoDeportivoID]) REFERENCES [dbo].[EventosDeportivos] ([ID])
);





