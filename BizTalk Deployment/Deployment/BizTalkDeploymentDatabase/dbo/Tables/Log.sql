CREATE TABLE [dbo].[Log](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Deployment] [bigint] NOT NULL,
	[Step] [int] NULL,
	[Time] [datetime] NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
) 
GO
ALTER TABLE [dbo].[Log]
    ADD CONSTRAINT [FK_Log_Deployment] FOREIGN KEY ([ID_Deployment]) REFERENCES [dbo].[Deployment] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;