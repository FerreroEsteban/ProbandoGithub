CREATE TABLE [dbo].[ApuestasDeportivas] (
    [ID]              INT          IDENTITY (1, 1) NOT NULL,
    [Codigo]          VARCHAR (10) NOT NULL,
    [Nombre]          NCHAR (10)   NOT NULL,
    [Activo]          BIT          NOT NULL,
    [ValorProveedor1] INT          NOT NULL,
    [ValorProveedor2] INT          NOT NULL,
    [ValorProveedor3] INT          NOT NULL,
    [ValorProveedor4] INT          NOT NULL,
    [Acualizado]      DATETIME     NOT NULL,
    [DeporteID]       INT          NOT NULL,
    CONSTRAINT [PK_ApuestasDeportivas] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [ApuestasDeportivas_FK1] FOREIGN KEY ([DeporteID]) REFERENCES [dbo].[Deportes] ([ID])
);

