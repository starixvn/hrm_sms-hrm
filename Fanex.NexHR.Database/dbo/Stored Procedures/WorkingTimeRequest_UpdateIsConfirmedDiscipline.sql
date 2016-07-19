CREATE PROCEDURE [dbo].[WorkingTimeRequest_UpdateIsConfirmedDiscipline]
AS
/*
Created: 20160719@Tim.Trinh
Task: Update IsConfirmed
DB: SMSHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EmployeeDiscipline (
		WorkDate DATE
		,EmployeeId INT
		,NumberOfDiscipline INT
		,NumberOfDisciplineConfirmed INT
		)

	INSERT INTO #EmployeeDiscipline
	SELECT DISTINCT dis.WorkDate
		,dis.EmpId
		,(
			CAST((CASE 
					WHEN ISNULL(dis.IsChangeShift, '') = '' THEN 0
					ELSE dis.IsChangeShift END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsMissingCheckInOut, '') = '' THEN 0
					ELSE dis.IsMissingCheckInOut END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsLateIn, '') = '' THEN 0
					ELSE dis.IsLateIn END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsEarlyOut, '') = '' THEN 0
					ELSE dis.IsEarlyOut END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsAbsent, '') = '' THEN 0
					ELSE dis.IsAbsent END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsUnpaidLeave, '') = '' THEN 0
					ELSE dis.IsUnpaidLeave END
					) AS INT)
			) AS NumberOfDiscipline
		,(
			CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedChangeShift, '') = '' THEN 0
					ELSE dis.IsConfirmedChangeShift END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedMissingCheckInOut, '') = '' THEN 0
					ELSE dis.IsConfirmedMissingCheckInOut END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedLateIn, '') = '' THEN 0
					ELSE dis.IsConfirmedLateIn END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedEarlyOut, '') = '' THEN 0
					ELSE dis.IsConfirmedEarlyOut END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedAbsent, '') = '' THEN 0
					ELSE dis.IsConfirmedAbsent END
					) AS INT) + CAST((CASE 
					WHEN ISNULL(dis.IsConfirmedUnpaidLeave, '') = '' THEN 0
					ELSE dis.IsConfirmedUnpaidLeave END
					) AS INT)
			) AS NumberOfDisciplineConfirmed
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
	WHERE dis.DisciplineType = 1

	UPDATE dis
	SET dis.IsConfirmed = (CASE 
			WHEN dis1.NumberOfDiscipline = dis1.NumberOfDisciplineConfirmed THEN 1
			ELSE 0 END
			)
	FROM dbo.EmployeeDiscipline AS dis
		INNER JOIN #EmployeeDiscipline AS dis1 ON dis.WorkDate = dis1.WorkDate
		AND dis.EmpId = dis1.EmployeeId
		AND dis.DisciplineType = 1
END