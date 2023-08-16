CREATE TABLE [dbo].[Manpower_PeriodProcessSetting] (
    [PeriodId]  SMALLINT NOT NULL,
    [ProcessId] TINYINT  NOT NULL,
    [FromDate]  DATE     NOT NULL,
    [ToDate]    DATE     NOT NULL,
    CONSTRAINT [PK_Manpower_PeriodProcessSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [ProcessId] ASC)
);

