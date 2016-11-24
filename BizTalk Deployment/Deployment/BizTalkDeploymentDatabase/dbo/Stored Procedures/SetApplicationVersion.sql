CREATE PROCEDURE dbo.SetApplicationVersion
	(
	@p_Application nvarchar(50),
	@p_Version nvarchar(50)
	)
AS

	UPDATE [dbo].[Application] WITH (ROWLOCK)
	   SET [Version] = @p_Version,
		   [Since] = getdate()
	 WHERE [Application] = @p_Application

	RETURN