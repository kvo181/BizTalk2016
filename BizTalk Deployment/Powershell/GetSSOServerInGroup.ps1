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
    -query:"SELECT [SSOServerName] FROM [BizTalkMgmtDb].[dbo].[adm_Group] WITH (nolock)" | 
        ForEach-Object { $_.SSOServerName }
