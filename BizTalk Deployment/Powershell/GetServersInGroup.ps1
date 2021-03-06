param 
( 
    [string]$server = "SomeServer", 
    [string]$database = "SomeDatabase"
)
if (!(Get-Alias -Name 'SQLQuery' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'SQLQuery' -Value "$BizTalkPSFolder\SQLQuery.ps1"
}


SQLQuery `
    -server:$server `
    -database:$database `
    -query:"SELECT [Name] FROM [BizTalkMgmtDb].[dbo].[adm_Server] WITH (nolock)" | 
        ForEach-Object { $_.Name }
