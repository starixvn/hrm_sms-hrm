CREATE TABLE [dbo].[EmployeeUploadFile] (
    [EmpId]        INT            NOT NULL,
    [FileName]     NVARCHAR (MAX) NOT NULL,
    [UploadedDate] DATETIME       NOT NULL,
    [IsDeleted]    BIT            NULL
);

