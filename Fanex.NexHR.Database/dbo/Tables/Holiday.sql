CREATE TABLE [dbo].[Holiday] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [HolidayName] NVARCHAR (50) NOT NULL,
    [FromDate]    DATETIME      NOT NULL,
    [ToDate]      DATETIME      NOT NULL,
    [TypeId]      TINYINT       NULL,
    CONSTRAINT [PK_NexHR_Holiday] PRIMARY KEY CLUSTERED ([Id] ASC) ON [EmpCommon]
) ON [EmpCommon];



