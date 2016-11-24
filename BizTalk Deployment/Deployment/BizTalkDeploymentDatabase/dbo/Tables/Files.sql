CREATE TABLE [dbo].[Files](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Package] [bigint] NOT NULL,
	[ResourceType] [nvarchar](256) NOT NULL,
	[Luid] [nvarchar](512) NOT NULL,
	[Filename] [nvarchar](512) NULL,
	[Version] [nvarchar](50) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
)
GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_Package] FOREIGN KEY ([ID_Package]) REFERENCES [dbo].[Package] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;