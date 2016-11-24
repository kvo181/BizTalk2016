CREATE PROCEDURE dbo.SetUninstallPackage 
	(
	@p_Group nvarchar(50)
	, @p_Application nvarchar(50)
	, @p_ProductCode nvarchar(50)
	, @p_UnInstallDateTime datetime
	, @p_UnInstallUser nvarchar(50)
	)
AS

	DECLARE @id_group BIGINT
	DECLARE @id_application bigint

	SELECT @id_group = [ID]
	  FROM [dbo].[Group] WITH (NOLOCK)
	  WHERE [Group] = @p_Group;
	SELECT @id_application = [ID]
		FROM [dbo].[Application] WITH (NOLOCK)
		WHERE [Application] = @p_Application;

	UPDATE [dbo].[Package] WITH (ROWLOCK)
         SET [UnInstalled] = 1
              ,[UnInstallDateTime] = @p_UnInstallDateTime
              ,[UnInstallUser] = @p_UnInstallUser
	  FROM [dbo].[Package] p 
      JOIN dbo.Deployment d
		ON d.ID = p.ID_Deployment
       WHERE p.[ID_Application] = @id_application
			AND p.ProductCode = @p_ProductCode
			AND d.ID_Group = @id_group
            AND ISNULL(p.[UnInstalled],0) = 0 ;
 
	RETURN