CREATE PROCEDURE dbo.InsertLogFile 
	(
	@p_id_deployment BIGINT,
	@p_info ntext
	)
AS

	INSERT INTO [dbo].[LogFile]
			   ([ID_Deployment]
			   ,[Info])
		 VALUES
			   (@p_id_deployment
			   ,@p_info)
           
	RETURN