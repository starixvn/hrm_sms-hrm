
CREATE PROCEDURE [dbo].[Postnatal_GetMember] @SupervisorId INT
	,@Query NVARCHAR(100)
AS
/*
Created: 20190506@Oliver.Dang
Task: Get member to add postnatal.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (ISNULL(@Query, '') <> '')
		SET @Query = '%' + @Query + '%'

	SELECT DISTINCT emp.EmpId As Id
		,emp.NickName
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE (emp.NickName LIKE '%' + @query + '%'
		OR emp.FullName LIKE '%' + @query + '%')
		AND emp.isDeleted = 0
		AND emp.[Enabled] = 1
		AND emp.Gender = 0
END