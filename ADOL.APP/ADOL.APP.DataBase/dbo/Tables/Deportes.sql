CREATE TABLE [dbo].[Deportes] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]        VARCHAR (100) NOT NULL,
    [Liga]          VARCHAR (10)  NOT NULL,
    [Pais]          VARCHAR (20)  NOT NULL,
    [Codigo]        VARCHAR (50)  NOT NULL,
    [Activo]        BIT           NOT NULL,
    [IDdeProveedor] VARCHAR (10)  NULL,
    [NombreInterno] VARCHAR (100) NULL,
    [NombePais]     VARCHAR (50)  NULL,
    CONSTRAINT [PK_Deportes] PRIMARY KEY CLUSTERED ([ID] ASC)
);





