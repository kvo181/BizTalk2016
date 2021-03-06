param 
( 
    [Parameter(Mandatory=$true)] 
    [string]$server, 
    [Parameter(Mandatory=$true)] 
    [string]$database,
    [Parameter(Mandatory=$true)] 
    [string]$application,
    [Parameter(Mandatory=$true)] 
    [string]$env,
    [Parameter(Mandatory=$false)] 
    [string]$version,
    [Parameter(Mandatory=$false)] 
	[string]$logfile
)

$currentLoc = Get-Location

Write-Host "Execute stop with:"
Write-Host "   server = $server"
Write-Host "   database = $database"
Write-Host "   application = $application"
Write-Host "   version = $version"
Write-Host "   env = $env"
Write-Host "   logfile = $logfile"

$startDate = Get-Date
Write-Host "On" $startDate

$currentLoc = Get-Location
$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

# Log start
$startLogDate = Get-Date -format yyyyMMddhhmmss -ErrorAction:Stop
if ($logfile -ne "") { 
    Add-Content $logfile -value "$application;;$username;$env;$startLogDate;12;stop $application" 
}

if ($server -eq "SomeServer") {
# get the group registration info
# the variables $MgmtDBName and $MgmtDBServer
.\GroupRegistrationInfo.ps1
if (!($MgmtDBServer)) {
    Set-Location $currentLoc
    Write-Error "No BizTalk group info available" 
    Throw "No BizTalk group info available" 
    exit
}
$server = $MgmtDBServer
$database = $MgmtDBName
}

# map the biztalk drive
if (!(get-pssnapin | where-object { $_.Name -eq 'BizTalkFactory.Powershell.Extensions' })) { 
    $InitializeDefaultBTSDrive = $false
    Add-PSSnapIn BizTalkFactory.PowerShell.Extensions
}
if (!(Get-PSDrive -Name BizTalk -ErrorAction:SilentlyContinue)) {
    New-PSDrive -Name BizTalk -Root BizTalk:\ -PSProvider BizTalk -Instance $server -Database $database -ErrorAction:Stop | Out-Null
}

# Get the list of dependent applications
if (!(Get-Alias -Name 'GetApplicationDependencies' -ErrorAction:SilentlyContinue)) {
Set-Alias -Name 'GetApplicationDependencies' -Value "$BizTalkToolsFolder\GetBTSApplicationDependencies.exe"
}

# Get the list of application referenced by this app
Set-Location BizTalk:\Applications -ErrorAction:Stop
$app = Get-ChildItem | Where-Object { $_.Name -eq $application }
if ($app -eq $null) {
	Set-Location $CurrentLoc
	$endDate = Get-Date
	Write-Host $application "was not deployed"
	Write-Host "Stop finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
	Write-Host ""
    if ($logfile -ne "") { 
        Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);18;stop finished for $application (was not deployed)" 
    }
	exit
}
if ($version -and $app.Description -ne $version) {
	Set-Location $currentLoc
	$endDate = Get-Date
	Write-Host "The deployed version" $app.Description "does not match the given version" $version "for the application" $application
	Write-Host "Stop finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
	Write-Host ""
    if ($logfile -ne "") { 
        Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);18;stop finished for $application (version mismatch)" 
    }
    exit
}
if ($app.Status -eq 'Stopped') {
	Set-Location $CurrentLoc
    $endDate = Get-Date
    Write-Host "The application" $application "is already stopped"
	Write-Host "Stop finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
    if ($logfile -ne "") { 
        Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);18;stop finished for $application (already stopped)" 
    }
	exit
}

# Get list list of dependent applications
Write-Host "Stopping the dependent applications for" $application 
Write-Host "Get Application Dependencies for" $application 
$apps = GetApplicationDependencies $application -server:$server -database:$database -ErrorAction:Stop
if ($apps.Trim().Length -gt 0) {

$Applications = $apps -Split ','
Write-Host ""
Write-Host "Found the following Application Dependencies: ("$Applications.Length")" $Applications 
Write-Host ""

# We need to stop the dependent applications first
$loopcnt = 0
while ($Applications -ne $null -and $Applications.length -ne 0 -and $loopcnt -le 50)
{
    # Errorhandling here
    trap [BizTalkFactory.Management.Automation.BtsException]
    {
        # Set parent variable to indicate an error has occured.
        Write-Host "Failed to stop application"
        Write-Error $_.Exception.Message
        Set-Variable -Name ErrorOccured -Value $true -Scope 1
        continue;
    }
   
    $ErrorOccured = $false
    
    # Prevent endless loop
    $loopcnt = $loopcnt + 1

    $app = Get-ChildItem | Where-Object { $_.Name -eq $Applications[0] }
    if ($app.Status -eq 'Stopped') {
        Write-Host $Applications[0] "already stopped"
        if ($logfile -ne "") { 
            Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);14;dependent stop finished for $Applications[0] (already stopped)" 
        }
    }
    else {
        # Stop the first item from the array.
        Write-Host "Trying to stop application: "  $Applications[0]
        if ($logfile -ne "") { 
            Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);13;start dependent stop for $Applications[0]" 
        }
        Stop-Application -Path $Applications[0] -StopOption UnenlistAllOrchestrations 
        Stop-Application -Path $Applications[0] -StopOption UnenlistAllSendPortGroups
        Stop-Application -Path $Applications[0] -StopOption UnenlistAllSendPorts
        Stop-Application -Path $Applications[0] -StopOption DisableAllReceiveLocations
        Stop-Application -Path $Applications[0] -StopOption UndeployAllPolicies
        if ($logfile -ne "") { 
            Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);14;dependent stop finished for $Applications[0]" 
        }
    }
    
    # Check if an error has occured.    
    if ($ErrorOccured)
    {
        # An error has occured. Put failed BtsApplication at the end of the array.
        $Applications = $Applications[1..($Applications.length - 1) + 0]
    }
    else
    {
        # Stop-Application was ok. Remove BtsApplication now also from the array or
        # if this was the last item set the array to null.
        if ($Applications.length -eq 1)
        {
            # Set the array to null.
            $Applications = $null
        }
        else
        {
            # Remove the item from the array.
            $Applications = $Applications[1..($Applications.length - 1)]
        }
    }
}

if ($loopcnt -gt 50) {
	Set-Location $CurrentLoc
    if ($logfile -ne "") { 
        Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);14;failed to stop the dependent applications" 
    }
    Throw "Failed to stop dependent applications for $application"
}

}
Write-Host "Done stopping the dependent applications for" $application 

# Stop the application
Write-Host "Stopping $application ..."
if ($logfile -ne "") { 
    Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);15;start stop for $application" 
}
Write-Host "UnenlistAllOrchestrations"
Stop-Application -Path $application -StopOption UnenlistAllOrchestrations -ErrorAction:SilentlyContinue -ErrorVariable:myError
if ($myError.Count -eq 0) {
    Write-Host "UnenlistAllSendPortGroups"
    Stop-Application -Path $application -StopOption UnenlistAllSendPortGroups -ErrorAction:SilentlyContinue -ErrorVariable:myError
}
if ($myError.Count -eq 0) {
    Write-Host "UnenlistAllSendPorts"
    Stop-Application -Path $application -StopOption UnenlistAllSendPorts -ErrorAction:SilentlyContinue -ErrorVariable:myError
}
if ($myError.Count -eq 0) {
    Write-Host "DisableAllReceiveLocations"
    Stop-Application -Path $application -StopOption DisableAllReceiveLocations -ErrorAction:SilentlyContinue -ErrorVariable:myError
}
if ($myError.Count -eq 0) {
    Write-Host "UndeployAllPolicies"
    Stop-Application -Path $application -StopOption UndeployAllPolicies -ErrorAction:SilentlyContinue -ErrorVariable:myError
}

Set-Location $CurrentLoc

if ($myError.Count -gt 0) {
    if ($logfile -ne "") { 
        Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);16;failed to stop the application : $application" 
    }
    Throw "Failed to stop $application : $myError[0]"
}

$endDate = Get-Date
Write-Host "Stop finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
Write-Host ""
if ($logfile -ne "") { 
    Add-Content $logfile -value "$application;;$username;$env;$(Get-Date -format yyyyMMddhhmmss);18;stop finished for $application" 
}
