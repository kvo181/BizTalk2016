CREATE TABLE [dbo].[Deployment](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Group] [bigint] NOT NULL,
	[User] [nvarchar](50) NULL,
	[DeploymentDate] [datetime] NULL,
	[ID_Application] [bigint] NOT NULL,
	[Version] [nvarchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[Start] [datetime] NULL,
	[End] [datetime] NULL,
	[Failed] [bit] NULL,
	[Error] [ntext] NULL,
	[Note] [nvarchar](512) NULL,
 CONSTRAINT [PK_Deployment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
) 
GO
ALTER TABLE [dbo].[Deployment]
    ADD CONSTRAINT [FK_Deployment_Application] FOREIGN KEY ([ID_Application]) REFERENCES [dbo].[Application] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [dbo].[Deployment]
    ADD CONSTRAINT [FK_Deployment_Group] FOREIGN KEY ([ID_Group]) REFERENCES [dbo].[Group] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;