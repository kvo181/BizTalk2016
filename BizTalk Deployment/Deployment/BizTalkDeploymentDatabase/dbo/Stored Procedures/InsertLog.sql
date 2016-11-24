CREATE PROCEDURE dbo.InsertLog 
	(
	@p_id_deployment BIGINT,
	@p_step INT,
	@p_datetime datetime,
	@p_description nvarchar(250)
	)
AS

	INSERT INTO [dbo].[Log]
			   ([ID_Deployment]
			   ,[Step]
			   ,[Time]
			   ,[Description])
		 VALUES
			   (@p_id_deployment
			   ,@p_step
			   ,@p_datetime
			   ,@p_description)
           
	RETURN