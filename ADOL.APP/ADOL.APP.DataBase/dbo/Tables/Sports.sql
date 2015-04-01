CREATE TABLE [dbo].[Sports] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [League]          VARCHAR (10)  NOT NULL,
    [Country]          VARCHAR (20)  NOT NULL,
    [Code]        VARCHAR (50)  NOT NULL,
    [Active]        BIT           NOT NULL,
    [ProviderID] VARCHAR (10)  NULL,
    [InternalName] VARCHAR (100) NULL,
    [CountryName]     VARCHAR (50)  NULL,
    CONSTRAINT [PK_Sports] PRIMARY KEY CLUSTERED ([ID] ASC)
);





