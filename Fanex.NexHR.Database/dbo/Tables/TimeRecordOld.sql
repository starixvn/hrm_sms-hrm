CREATE TABLE [dbo].[TimeRecordOld] (
    [Id]           INT      NOT NULL,
    [EmpID]        INT      NOT NULL,
    [RecordTime]   DATETIME NOT NULL,
    [InOutType]    TINYINT  NULL,
    [InsertDate]   DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL
);

