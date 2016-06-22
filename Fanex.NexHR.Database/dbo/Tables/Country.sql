CREATE TABLE [dbo].[Country] (
    [CountryCode]  VARCHAR (5)   NULL,
    [CountryName]  VARCHAR (50)  NOT NULL,
    [CountryOrder] INT           NULL,
    [Nationality]  VARCHAR (150) NULL
) ON [EmpCommon];

