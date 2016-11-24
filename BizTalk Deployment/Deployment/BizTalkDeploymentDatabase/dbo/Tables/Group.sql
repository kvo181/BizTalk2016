CREATE TABLE [dbo].[Group](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Group] [nchar](50) NULL,
	[Environment] [nchar](50) NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) 
) 