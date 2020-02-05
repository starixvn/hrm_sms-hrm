
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/19/2016 5:22:11 PM ******/
CREATE PROCEDURE [dbo].[EmployeeRelationship_Edit] @RelationshipId INT
	,@FullName NVARCHAR(200)
	,@Phone NVARCHAR(100)
	,@Relationship NVARCHAR(50)
	,@DateOfBirth DATE
	,@Gender BIT
	,@IsEmergency BIT
	,@Modifier INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update relationship.
DB: NexEPA
Original: 

Revisions:  
	- 20200205@MerlinLe: Add field [Relationship] and [IsEmergency]
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRelationship WITH (NOLOCK)
				WHERE Id = @RelationshipId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The relationship not found'

		RETURN
	END

	UPDATE dbo.EmployeeRelationship
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET FullName = @FullName
		,Phone = @Phone
		,Relationship = @Relationship
		,DateOfBirth = @DateOfBirth
		,Gender = @Gender
		,IsEmergency = @IsEmergency
		,ModifiedBy = @Modifier
		,ModifiedDate = GETDATE()
	WHERE Id = @RelationshipId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update relationship failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END