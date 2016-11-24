CREATE TABLE [dbo].[LogFile]
(
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Deployment] [bigint] NOT NULL,
	[Info] [NTEXT] NOT NULL
 CONSTRAINT [PK_LogFile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
) 
GO
ALTER TABLE [dbo].[LogFile]
    ADD CONSTRAINT [FK_LogFile_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;