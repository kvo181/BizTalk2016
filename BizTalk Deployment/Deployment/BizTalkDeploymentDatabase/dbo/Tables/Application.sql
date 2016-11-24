CREATE TABLE [dbo].[Application](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Version] [nvarchar](50) NULL,
	[Since] [datetime] NULL,
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
) 