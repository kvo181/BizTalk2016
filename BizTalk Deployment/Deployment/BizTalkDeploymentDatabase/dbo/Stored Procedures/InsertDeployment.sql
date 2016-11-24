CREATE PROCEDURE dbo.InsertDeployment 
	(
	@p_Group nvarchar(50),
	@p_Environment nvarchar(50),
	@p_User nvarchar(50),
	@p_Date datetime,
	@p_Application nvarchar(50),
	@p_Version nvarchar(50),
	@p_Action nvarchar(50),
	@p_Note nvarchar(512),
	@p_id_deployment BIGINT OUTPUT
	)
AS

	SET NOCOUNT ON

	DECLARE @id_group BIGINT
	DECLARE @id_application BIGINT

	-- get or create group
	SELECT @id_group = [ID]
	  FROM [dbo].[Group] WITH (NOLOCK)
	  WHERE [Group] = @p_Group;
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [dbo].[Group]
				   ([Group]
				   ,[Environment])
			 VALUES
				   (@p_Group
				   ,@p_Environment)
		SET @id_group = @@IDENTITY
	END

	-- get or create application
	SELECT @id_application = [ID]
	  FROM [dbo].[Application] WITH (NOLOCK)
	  WHERE [Application] = @p_Application;
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [dbo].[Application]
				   ([Application])
			 VALUES
				   (@p_Application)
		SET @id_application = @@IDENTITY
	END
	           
	INSERT INTO [dbo].[Deployment]
			   ([ID_Group]
			   ,[User]
			   ,[DeploymentDate]
			   ,[ID_Application]
			   ,[Version]
			   ,[Action]
			   ,[Start]
			   ,[Note])
		 VALUES
			   (@id_group
			   ,@p_User
			   ,@p_Date
			   ,@id_application
			   ,@p_Version
			   ,@p_Action
			   ,@p_Date
			   ,@p_Note)

	SET @p_id_deployment = @@IDENTITY
           
	RETURN