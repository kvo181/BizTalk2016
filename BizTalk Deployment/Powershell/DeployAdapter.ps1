
param 
( 
    [Parameter(Mandatory=$true)] 
    [string]$server = "SomeServer", 
    [Parameter(Mandatory=$true)] 
    [string]$database = "SomeDatabase",
    [Parameter(Mandatory=$true)] 
    [string]$msi = "SomeApp.msi"
)

Get-Date
Write-Host "Execute deploy adapter with:"
Write-Host "   server = $server"
Write-Host "   database = $database"
Write-Host "   msi = $msi"

$startDate = Get-Date -ErrorAction:Stop
Write-Host "On Date:" $startDate
Write-Host "On Server:" $env:computername

$file = (Get-ChildItem "$msi" -ErrorAction:Stop).Name

$currentLoc = Get-Location
$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

$computername = gc env:computername

if ($server -eq "SomeServer" -or $server -eq "" -or 
    $database -eq "SomeDatabase" -or $database -eq "" -or 
    $msi -eq "SomeApp.msi" -or $msi -eq "") {
    Write-Host "Not all parameters were passed correctly!"
    exit
}

# Get the credentials
$cred = $host.ui.PromptForCredential("Need credentials", "Please enter your user name and password.", "", "")
if ($cred -eq $null) { exit }

$startDate = Get-Date -ErrorAction:Stop

$username = $cred.UserName
$startLogDate = Get-Date -format yyyyMMddhhmmss -ErrorAction:Stop

# GroupAvailable
if (!(Get-Alias -Name 'GroupAvailable' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GroupAvailable' -Value "$BizTalkPSFolder\GroupAvailable.ps1"
}
# GetServersInGroup
if (!(Get-Alias -Name 'GetServersInGroup' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GetServersInGroup' -Value "$BizTalkPSFolder\GetServersInGroup.ps1"
}
# CreateLocalFolder
if (!(Get-Alias -Name 'CreateLocalFolder' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'CreateLocalFolder' -Value "$BizTalkPSFolder\CreateLocalFolder.ps1"
}
# InstallAdapter
if (!(Get-Alias -Name 'InstallAdapter' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'InstallAdapter' -Value "$BizTalkPSFolder\InstallAdapter.ps1"
}

# We need to install this adapter

# Before we continue, the group has to be operational 
# (all registrered servers must be up and running)
Write-Host "Check availability for group on" $server "..."
if (!(GroupAvailable -server:$server -database:$database)) {
    Write-Error "Deployment to $server and $database is not possible" 
    exit
}

# Get the servers belonging to the group
Write-Host "Get application servers for" $server "..."
$servers = GetServersInGroup -server $server -database $database
if ($servers.GetType().Name -eq "String") {
$appserver = $servers + ".$BizTalkDomain"
} else {
$appserver = $servers[0] + ".$BizTalkDomain" 
}

# Install the adapter on every server listed in the group
foreach ($s in $servers) { 

  $appserver = $s + ".$BizTalkDomain"

  # Deploy locally or not
  $deployLocal = $false
  if ($env:computername.ToLower() -eq $s.ToLower()) { $deployLocal = $true }

  Write-Host "Prepare install on" $appserver "..."
  $folder = "\\" + $appserver + "\$BizTalkTmpInstall"

  if ($deployLocal) {
  CreateLocalFolder $folder -ErrorAction:Stop
  }
  else {
  Invoke-Command -ComputerName $appserver -FilePath "$BizTalkPSFolder\CreateLocalFolder.ps1" -ArgumentList $folder -ConfigurationName Microsoft.Powershell32 -Authentication CredSSP -Credential $cred -ErrorAction:Stop
  }

  $localmsi = $folder + "\" + $file
  Copy-Item "$msi" -Destination "$localmsi" -Force -ErrorAction:Stop
  if (!(Test-Path -Path "$localmsi" -PathType Leaf)) {
      Write-Host $localmsi "could not be located!"
      exit
  }

  Write-Host "Start install on" $appserver "..."
  if ($deployLocal) {
  InstallAdapter "$localmsi" $cred -ErrorAction:Stop
  }
  else {
  Invoke-Command -ComputerName $appserver -FilePath "$BizTalkPSFolder\InstallAdapter.ps1" -ArgumentList "$localmsi",$cred -ConfigurationName Microsoft.Powershell32 -AsJob -JobName InstallAdapter -Authentication CredSSP -Credential $cred | Out-Null
  }
  
}

if (!$deployLocal) {  
# Wait for it all to complete 
While (Get-Job -State "Running") 
{ 
  Start-Sleep 1 
} 
  
# Getting the information back from the jobs 
$abortdeploy = $false
Write-Host "Install finished"
$results = Receive-Job -Name InstallAdapter
if ($results -ne $null) {
    #Look for ExitCode= 0
    #if ($results.ToString() -match "ExitCode") { $abortdeploy = $true }
}
Write-Host ""
if ($abortdeploy) {
    Set-Location $currentLoc
    # Log failure of deployment
    #if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);99;Install failed" }
    $Host.UI.WriteErrorLine("Install adapter failed")
    throw "Install adapter failed for '$msi'"
    exit
}
}

#Write-Host "Register the $adapter adapter"
#"c:\BizTalk\Adapters\$adapter\RegisterAdapter.exe"
# Ready to register the adapter
#if ($LASTEXITCODE -ne 0) {
#    Write-Error "Register of the $adapter adapter failed!"
#}

Write-Host "Start cleanup ..."
foreach ($s in $servers) { 
  $appserver = $s + ".$BizTalkDomain"
  $folder = "\\" + $appserver + "\$BizTalkTmpInstall\" + $application
  $localmsi = $folder + "\" + $file
  Write-Host "Removing $localmsi ..."
  Remove-Item "$localmsi" -Force
}
Write-Host ""
Write-Host "Deployment finished after" (Get-Date).subtract($startdate).TotalSeconds "seconds"
