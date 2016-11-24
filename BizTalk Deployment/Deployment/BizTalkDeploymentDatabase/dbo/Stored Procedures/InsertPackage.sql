CREATE PROCEDURE dbo.InsertPackage 
	(
	@p_ID_Deployment bigint
	, @p_Application nvarchar(50)
	, @p_PackageCode nvarchar(50)
	, @p_ProductCode nvarchar(50)
	, @p_Title nvarchar(256)
	, @p_Author nvarchar(256)
	, @p_Subject nvarchar(256)
	, @p_Comments nvarchar(256)
	, @p_Keywords nvarchar(256)
	, @p_CreateTime nvarchar(256)
	, @p_id_package BIGINT OUTPUT
	)
AS

	DECLARE @id_application bigint
	
	SELECT @id_application = [ID]
		FROM [dbo].[Application]
		WHERE [Application] = @p_Application;

	INSERT INTO [dbo].[Package]
			   ([ID_Deployment]
			   ,[PackageCode]
			   ,[ProductCode]
			   ,[ID_Application]
			   ,[Title]
			   ,[Author]
			   ,[Subject]
			   ,[Comments]
			   ,[Keywords]
			   ,[CreateTime])
		 VALUES
			   (@p_ID_Deployment
			   ,@p_PackageCode
			   ,@p_ProductCode
			   ,@id_application
			   ,@p_Title
			   ,@p_Author
			   ,@p_Subject
			   ,@p_Comments
			   ,@p_Keywords
			   ,@p_CreateTime)
           
	SET @p_id_package = @@IDENTITY

	RETURN