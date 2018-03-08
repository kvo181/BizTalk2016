﻿/*
Deployment script for BizTalkDeploymentDb

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "BizTalkDeploymentDb"
:setvar DefaultFilePrefix "BizTalkDeploymentDb"
:setvar DefaultDataPath "D:\Program Files\Microsoft SQL Server\MSSQL13.BIZTALK\MSSQL\DATA\"
:setvar DefaultLogPath "L:\Program Files\Microsoft SQL Server\MSSQL13.BIZTALK\MSSQL\Data\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [master];


GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [$(DatabaseName)], FILENAME = N'$(DefaultDataPath)$(DefaultFilePrefix)_Primary.mdf')
    LOG ON (NAME = [$(DatabaseName)_log], FILENAME = N'$(DefaultLogPath)$(DefaultFilePrefix)_Primary.ldf') COLLATE SQL_Latin1_General_CP1_CI_AS
GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET TARGET_RECOVERY_TIME = 0 SECONDS 
    WITH ROLLBACK IMMEDIATE;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF),
                CONTAINMENT = NONE 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CREATE_STATISTICS ON(INCREMENTAL = OFF),
                MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT = OFF,
                DELAYED_DURABILITY = DISABLED 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE (QUERY_CAPTURE_MODE = ALL, FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_PLANS_PER_QUERY = 200, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 367), MAX_STORAGE_SIZE_MB = 100) 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE = OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
    END


GO
USE [$(DatabaseName)];


GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
PRINT N'Creating [dbo].[Application]...';


GO
CREATE TABLE [dbo].[Application] (
    [ID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [Application] NVARCHAR (50) NOT NULL,
    [Version]     NVARCHAR (50) NULL,
    [Since]       DATETIME      NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[Deployment]...';


GO
CREATE TABLE [dbo].[Deployment] (
    [ID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [ID_Group]       BIGINT         NOT NULL,
    [User]           NVARCHAR (50)  NULL,
    [DeploymentDate] DATETIME       NULL,
    [ID_Application] BIGINT         NOT NULL,
    [Version]        NVARCHAR (50)  NULL,
    [Action]         NVARCHAR (50)  NULL,
    [Start]          DATETIME       NULL,
    [End]            DATETIME       NULL,
    [Failed]         BIT            NULL,
    [Error]          NTEXT          NULL,
    [Note]           NVARCHAR (512) NULL,
    CONSTRAINT [PK_Deployment] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[Files]...';


GO
CREATE TABLE [dbo].[Files] (
    [ID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [ID_Package]   BIGINT         NOT NULL,
    [ResourceType] NVARCHAR (256) NOT NULL,
    [Luid]         NVARCHAR (512) NOT NULL,
    [Filename]     NVARCHAR (512) NULL,
    [Version]      NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[Group]...';


GO
CREATE TABLE [dbo].[Group] (
    [ID]          BIGINT     IDENTITY (1, 1) NOT NULL,
    [Group]       NCHAR (50) NULL,
    [Environment] NCHAR (50) NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[Log]...';


GO
CREATE TABLE [dbo].[Log] (
    [ID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [ID_Deployment] BIGINT         NOT NULL,
    [Step]          INT            NULL,
    [Time]          DATETIME       NULL,
    [Description]   NVARCHAR (250) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[LogFile]...';


GO
CREATE TABLE [dbo].[LogFile] (
    [ID]            BIGINT IDENTITY (1, 1) NOT NULL,
    [ID_Deployment] BIGINT NOT NULL,
    [Info]          NTEXT  NOT NULL,
    CONSTRAINT [PK_LogFile] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[Package]...';


GO
CREATE TABLE [dbo].[Package] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ID_Deployment]     BIGINT         NOT NULL,
    [PackageCode]       NVARCHAR (50)  NOT NULL,
    [ProductCode]       NVARCHAR (50)  NULL,
    [ID_Application]    BIGINT         NULL,
    [Title]             NVARCHAR (256) NULL,
    [Author]            NVARCHAR (256) NULL,
    [Subject]           NVARCHAR (256) NULL,
    [Comments]          NVARCHAR (256) NULL,
    [Keywords]          NVARCHAR (256) NULL,
    [CreateTime]        NVARCHAR (256) NULL,
    [UnInstalled]       BIT            NULL,
    [UnInstallDateTime] DATETIME       NULL,
    [UnInstallUser]     NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[FK_Deployment_Application]...';


GO
ALTER TABLE [dbo].[Deployment]
    ADD CONSTRAINT [FK_Deployment_Application] FOREIGN KEY ([ID_Application]) REFERENCES [dbo].[Application] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Deployment_Group]...';


GO
ALTER TABLE [dbo].[Deployment]
    ADD CONSTRAINT [FK_Deployment_Group] FOREIGN KEY ([ID_Group]) REFERENCES [dbo].[Group] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Files_Package]...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_Package] FOREIGN KEY ([ID_Package]) REFERENCES [dbo].[Package] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Log_Deployment]...';


GO
ALTER TABLE [dbo].[Log]
    ADD CONSTRAINT [FK_Log_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]);


GO
PRINT N'Creating [dbo].[FK_LogFile_Deployment]...';


GO
ALTER TABLE [dbo].[LogFile]
    ADD CONSTRAINT [FK_LogFile_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Package_Application]...';


GO
ALTER TABLE [dbo].[Package]
    ADD CONSTRAINT [FK_Package_Application] FOREIGN KEY ([ID_Application]) REFERENCES [dbo].[Application] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Package_Deployment]...';


GO
ALTER TABLE [dbo].[Package]
    ADD CONSTRAINT [FK_Package_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]);


GO
PRINT N'Creating [dbo].[InsertDeployment]...';


GO
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
GO
PRINT N'Creating [dbo].[InsertFile]...';


GO
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
GO
PRINT N'Creating [dbo].[InsertLog]...';


GO
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
GO
PRINT N'Creating [dbo].[InsertLogFile]...';


GO
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
GO
PRINT N'Creating [dbo].[InsertPackage]...';


GO
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
GO
PRINT N'Creating [dbo].[SetApplicationVersion]...';


GO
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
GO
PRINT N'Creating [dbo].[SetEndDeployment]...';


GO
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
GO
PRINT N'Creating [dbo].[SetUninstallPackage]...';


GO
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
GO
DECLARE @VarDecimalSupported AS BIT;

SELECT @VarDecimalSupported = 0;

IF ((ServerProperty(N'EngineEdition') = 3)
    AND (((@@microsoftversion / power(2, 24) = 9)
          AND (@@microsoftversion & 0xffff >= 3024))
         OR ((@@microsoftversion / power(2, 24) = 10)
             AND (@@microsoftversion & 0xffff >= 1600))))
    SELECT @VarDecimalSupported = 1;

IF (@VarDecimalSupported > 0)
    BEGIN
        EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
    END


GO
PRINT N'Update complete.';


GO