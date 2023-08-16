CREATE TABLE [dbo].[Manpower_Flow] (
    [UnitName]  NVARCHAR (200) NOT NULL,
    [ProcessId] TINYINT        NOT NULL,
    [SiteId]    SMALLINT       NOT NULL,
    [EmpId]     INT            NOT NULL,
    CONSTRAINT [PK_Manpower_Flow] PRIMARY KEY CLUSTERED ([UnitName] ASC, [ProcessId] ASC, [SiteId] ASC, [EmpId] ASC)
);

