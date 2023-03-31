CREATE TABLE [dbo].[TimeRecord] (
    [ID]           INT      IDENTITY (1, 1) NOT NULL,
    [EmpID]        INT      NOT NULL,
    [RecordTime]   DATETIME NOT NULL,
    [InOutType]    TINYINT  NULL,
    [InsertDate]   DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_TimeRecord] PRIMARY KEY CLUSTERED ([ID] ASC) ON [EmpAttendance]
) ON [EmpAttendance];




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'IN record (0) or OUT record (1)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TimeRecord', @level2type = N'COLUMN', @level2name = N'InOutType';


GO
CREATE NONCLUSTERED INDEX [IDX_TimeRecord_EmpId_Time]
    ON [dbo].[TimeRecord]([EmpID] ASC, [RecordTime] ASC)
    ON [EmpAttendance];

