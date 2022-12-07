
CREATE PROCEDURE [dbo].[Log_Detail_Insert] @UserId INT
	,@SiteId SMALLINT = NULL
	,@SpName VARCHAR(50) = NULL
	,@ObjectId INT = NULL
	,@Data NVARCHAR(MAX) = NULL
	,@OldData NVARCHAR(MAX) = NULL
	,@LogDate DATETIME = NULL
	,@Duration INT = NULL
AS
/*
Created: 20220113@Tim.Trinh
Task: write log
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Log_Detail (
		UserId
		,SiteId
		,SpName
		,ObjectId
		,LogDate
		,Duration
		,Data
		,OldData
		)
	VALUES (
		@UserId
		,@SiteId
		,@SpName
		,@ObjectId
		,@LogDate
		,@Duration
		,@Data
		,@OldData
		)
END