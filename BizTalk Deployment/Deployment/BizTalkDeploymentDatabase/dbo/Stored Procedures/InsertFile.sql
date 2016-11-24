CREATE PROCEDURE dbo.InsertFile 
	(
	@p_ID_Package bigint
	, @p_ResourceType nvarchar(256)
	, @p_Luid nvarchar(512)
	, @p_Filename nvarchar(512)
	, @p_Version nvarchar(50)
	)
AS

	INSERT INTO [dbo].[Files]
			   ([ID_Package]
			   ,[ResourceType]
			   ,[Luid]
			   ,[Filename]
			   ,[Version])
		 VALUES
			   (@p_ID_Package
			   ,@p_ResourceType
			   ,@p_Luid
			   ,@p_Filename
			   ,@p_Version)

	RETURN