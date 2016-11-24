CREATE PROCEDURE dbo.SetEndDeployment 
	(
	@p_id_deployment BIGINT,
	@p_date datetime,
	@p_failed bit,
	@p_Error ntext
	)
AS

	UPDATE [dbo].[Deployment] WITH (ROWLOCK)
	   SET [End] = @p_date, [Failed] = @p_failed, [Error] = @p_Error 
	 WHERE ID = @p_id_deployment

	RETURN