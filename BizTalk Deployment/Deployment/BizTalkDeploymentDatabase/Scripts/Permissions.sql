
GRANT CONNECT TO [BizTalk Server Administrators]
    AS [dbo];
GO

GRANT CONNECT TO [dbo]
    AS [dbo];
GO

GRANT EXECUTE
    ON SCHEMA::[dbo] TO [BizTalk Server Administrators]
    AS [dbo];
GO

GRANT SELECT
    ON SCHEMA::[dbo] TO [BTS_USER]
    AS [dbo];
GO

GRANT SELECT
    ON SCHEMA::[dbo] TO [BizTalk Server Administrators]
    AS [dbo];
GO
