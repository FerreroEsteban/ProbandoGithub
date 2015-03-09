﻿CREATE TABLE [dbo].[EventosDeportivos] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]    NVARCHAR (100) NOT NULL,
    [Codigo]    VARCHAR (50)   NOT NULL,
    [Activo]    BIT            NOT NULL,
    [Inicio]    DATETIME       NOT NULL,
    [Fin]       DATETIME       NOT NULL,
    [DeporteID] INT            NOT NULL,
    CONSTRAINT [PK_EventosDeportivos] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [EventosDeportivos_FK1] FOREIGN KEY ([DeporteID]) REFERENCES [dbo].[Deportes] ([ID])
);
