CREATE TABLE [dbo].[Package](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Deployment] [bigint] NOT NULL,
	[PackageCode] [nvarchar](50) NOT NULL,
	[ProductCode] [nvarchar](50) NULL,
	[ID_Application] [bigint] NULL,
	[Title] [nvarchar](256) NULL,
	[Author] [nvarchar](256) NULL,
	[Subject] [nvarchar](256) NULL,
	[Comments] [nvarchar](256) NULL,
	[Keywords] [nvarchar](256) NULL,
	[CreateTime] [nvarchar](256) NULL,
	[UnInstalled] [bit] NULL,
	[UnInstallDateTime] [datetime] NULL,
	[UnInstallUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
)
GO
ALTER TABLE [dbo].[Package]
    ADD CONSTRAINT [FK_Package_Application] FOREIGN KEY ([ID_Application]) REFERENCES [dbo].[Application] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [dbo].[Package]
    ADD CONSTRAINT [FK_Package_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;