param 
( 

    [Parameter(Mandatory=$true)] 
    [string]$server, 
    [Parameter(Mandatory=$true)] 
    [string]$database,
    [Parameter(Mandatory=$true)] 
    [string]$application,
    [Parameter(Mandatory=$true)] 
    [string]$env
)

$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

# Get the credentials
$cred = $host.ui.PromptForCredential("Need credentials", "Please enter your user name and password.", $username, "")
if ($cred -eq $null) { exit }

$username = $cred.UserName

# GetServersInGroup
if (!(Get-Alias -Name 'GetServersInGroup' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GetServersInGroup' -Value "$BizTalkPSFolder\GetServersInGroup.ps1"
}

# Get the servers belonging to the group
Write-Host "Get application servers for" $server "..."
$servers = GetServersInGroup -server $server -database $database
if ($servers.GetType().Name -eq "String"){
$appserver = $servers + ".$BizTalkDomain"
} else {
$appserver = $servers[0] + ".$BizTalkDomain" }

Invoke-Command -ComputerName $appserver -FilePath "$BizTalkPSFolder\StartApplication.ps1" -ArgumentList $server, $database, $application, $env  -ConfigurationName Microsoft.Powershell32 -Authentication CredSSP -Credential $cred | Out-Null
