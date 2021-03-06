
param 
( 
    [Parameter(Mandatory=$true)] 
    [string]$server, 
    [Parameter(Mandatory=$true)] 
    [string]$database,
    [Parameter(Mandatory=$true)] 
    [string]$application,
    [Parameter(Mandatory=$true)] 
    [string]$msi,
    [Parameter(Mandatory=$false)] 
    [string]$env,
    [Parameter(Mandatory=$false)] 
	[string]$logfile
)

Get-Date
Write-Host "Execute deploy with:"
Write-Host "   server = $server"
Write-Host "   database = $database"
Write-Host "   application = $application"
Write-Host "   msi = $msi"
Write-Host "   env = $env"
Write-Host "   logfile = $logfile"

$startDate = Get-Date -ErrorAction:Stop
Write-Host "On Date:" $startDate
Write-Host "On Server:" $env:computername

$file = (Get-ChildItem "$msi" -ErrorAction:Stop).Name

$currentLoc = Get-Location
$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

# Get the credentials
$cred = $host.ui.PromptForCredential("Need credentials", "Please enter your user name and password.", $username, "")
if ($cred -eq $null) { exit }

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
# GetSSOServerInGroup
if (!(Get-Alias -Name 'GetSSOServerInGroup' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GetSSOServerInGroup' -Value "$BizTalkPSFolder\GetSSOServerInGroup.ps1"
}
# GetUninstallPackage
if (!(Get-Alias -Name 'GetUninstallPackage' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GetUninstallPackage' -Value "$BizTalkToolsFolder\GetUninstallPackage.exe"
}
# CreateLocalFolder
if (!(Get-Alias -Name 'CreateLocalFolder' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'CreateLocalFolder' -Value "$BizTalkPSFolder\CreateLocalFolder.ps1"
}
# InstallApplication
if (!(Get-Alias -Name 'InstallApplication' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'InstallApplication' -Value "$BizTalkPSFolder\InstallApplication.ps1"
}

# We need to import and install this app

# Before we continue, the group has to be operational 
# (all registrered servers must be up and running)
Write-Host "Check availability for group on" $server "..."
if (!(GroupAvailable -server:$server -database:$database)) {
    Write-Error "Deployment to $server and $database is not possible" 
    Set-Location $currentLoc
    exit
}

# Log start
if ($logfile -ne "") { 
    New-Item $logfile -type file -force
    Add-Content $logfile -value "$application;$msi;$username;$env;$startLogDate;1;start install of $msi" 
}

# Get the servers belonging to the group
Write-Host "Get application servers for" $server "..."
$servers = GetServersInGroup -server $server -database $database
if ($servers.GetType().Name -eq "String") {
$appserver = $servers + ".$BizTalkDomain"
} else {
$appserver = $servers[0] + ".$BizTalkDomain" }

# Log after get servers
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);2;after get servers" }

$uninstallpackages = GetUninstallPackage $appserver $application
foreach ($uninstallpackage in $uninstallpackages) {
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);11;uninstalled $uninstallpackage" }
}

# Install the application on every server listed in the group
# Has to be done before the import since may need post-processing script files during import
foreach ($s in $servers) { 

  $appserver = $s + ".$BizTalkDomain"

  # Deploy locally or not
  $deployLocal = $false
  if ($env:computername.ToLower() -eq $s.ToLower()) { $deployLocal = $true }

  Write-Host "Prepare install on" $appserver "..."
  $folder = "\\" + $appserver + "\$BizTalkTmpInstall\" + $application
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
      Set-Location $currentLoc
      throw "$localmsi could not be located!"
      exit
  }

  # Log after prepare install
  if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);3;install prepared on $appserver" }

  Write-Host "Start install on" $appserver "..."
  # Define what each job does 
  if ($deployLocal) {
  InstallApplication $application "$localmsi" -ErrorAction:Stop
  }
  else {
  Invoke-Command -ComputerName $appserver -FilePath "$BizTalkPSFolder\InstallApplication.ps1" -ArgumentList $application, "$localmsi" -ConfigurationName Microsoft.Powershell32 -AsJob -JobName InstallApp -Authentication CredSSP -Credential $cred | Out-Null
  }

    # Log after prepare install
  if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);4;install started on $appserver" }

}
  
# Wait for it all to complete 
While (Get-Job -State "Running") 
{ 
  Start-Sleep 1 
} 
  
# Getting the information back from the jobs 
$abortdeploy = $false
Write-Host "Install finished"
$results = Receive-Job -Name InstallApp -ErrorAction:SilentlyContinue
if ($results -ne $null) {
    #Look for ExitCode= 0
    if ($results.ToString() -match "ExitCode") { $abortdeploy = $true }
}
Write-Host ""
if ($abortdeploy) {
    Set-Location $currentLoc
    # Log failure of deployment
    if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);99;Install failed" }
    $Host.UI.WriteErrorLine("Install during Deploy failed")
    throw "Install during Deploy failed for '$msi'"
    exit
}

# Log after install finished
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);5;install finished" }

Write-Host "Start cleanup ..."
foreach ($s in $servers) { 
  $appserver = $s + ".$BizTalkDomain"
  $folder = "\\" + $appserver + "\$BizTalkTmpInstall\" + $application
  $localmsi = $folder + "\" + $file
  Write-Host "Removing $localmsi ..."
  Remove-Item $localmsi -Force
  # Log after cleanup finished
  if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);6;cleanup finished on $appserver" }
}
Write-Host ""

# Deploy locally or not
$deployLocal = $false
# Where to import (on the first registrered server of the group)
if ($servers.GetType().Name -eq "String") {
if ($env:computername.ToLower() -eq $servers.ToLower()) { $deployLocal = $true }
$appserver = $servers + ".$BizTalkDomain"
} else {
if ($env:computername.ToLower() -eq $servers[0].ToLower()) { $deployLocal = $true }
$appserver = $servers[0] + ".$BizTalkDomain" }

# Log start Undeploy
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);7;start undeploy on $appserver" }

# UnDeploy application
if (!(Get-Alias -Name 'UnDeployApplication' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'UnDeployApplication' -Value "$BizTalkPSFolder\UnDeployApplication.ps1"
}

# map the biztalk drive
if (!(get-pssnapin | where-object { $_.Name -eq 'BizTalkFactory.Powershell.Extensions' })) { 
    $InitializeDefaultBTSDrive = $false
    Add-PSSnapIn BizTalkFactory.PowerShell.Extensions
}
if (!(Get-PSDrive -Name BizTalk -ErrorAction:SilentlyContinue)) {
    New-PSDrive -Name BizTalk -Root BizTalk:\ -PSProvider BizTalk -Instance $server -Database $database -ErrorAction:Stop | Out-Null
}

# Is the application available?
Set-Location BizTalk:\Applications -ErrorAction:Stop
$app = Get-ChildItem | Where-Object { $_.Name -eq $application }
if ($app -ne $null) {
    Set-Location $currentLoc
    # We need to undeploy it first!
    Write-Host "Invoke undeploy on" $app.Name "..."
    UnDeployApplication -server:$server -database:$database -application:$application -env:$env -logfile:$logfile -ErrorAction:SilentlyContinue -ErrorVariable:myError
    if ($myError -ne $null) {
        if ($myError.Count -gt 0) {
            Set-Location $currentLoc
            # Log failure of deployment
            if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);98;Remove failed : $myError[0]" }
            $Host.UI.WriteErrorLine($myError[0])
            Throw "Failed to undeploy $application : $myError[0]"
            exit
        }
    }
}

# Log start import
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);50;start import on $appserver" }

# Import application
if (!(Get-Alias -Name 'ImportApplication' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'ImportApplication' -Value "$BizTalkPSFolder\ImportApplication.ps1"
}

# Import the application in the group
Write-Host "Invoke import in" $server "on" $appserver "..."
#if ($deployLocal) {
#    ImportApplication -server:$server -database:$database -application:$application -msi:$msi -env:$env -ErrorAction:SilentlyContinue -ErrorVariable:myError
#}
#else {
    Invoke-Command -ComputerName:$appserver -FilePath:"$BizTalkPSFolder\ImportApplication.ps1" -ArgumentList:$server, $database, $application, $msi, $env -ConfigurationName:Microsoft.Powershell32 -Authentication:CredSSP -Credential:$cred -ErrorAction:SilentlyContinue -ErrorVariable:myError
#}
if ($myError -ne $null) {
    if ($myError.Count -gt 0) {
    # Log failure of deployment
    if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);99;Import failed $myError[0]" }
    $Host.UI.WriteErrorLine($myError[0])
    throw "Import on server $appserver failed for '$msi'"
    exit
    }
}

# Deploy SSO
# Get the SSO server for this group
$ssoserver = GetSSOServerInGroup -server $server -database $database
#watch out when SSO is configured on a SQL cluster: if ($ssoserver -eq 'BTSSSOCLUPRD') { $ssoserver = "lobizsqlprd01" }
$appssoserver = $ssoserver + ".$BizTalkDomain"

# Log start deploy sso
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);55;start deploy sso on $appssoserver" }

# Deploy SSO
if (!(Get-Alias -Name 'DeploySSO' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'DeploySSO' -Value "$BizTalkPSFolder\DeploySSO.ps1"
}

if ($BizTalkInstallFolder -eq $null) { $BizTalkInstallFolder = "\\$appserver\C$\Program Files (x86)\Generated by BizTalk" }
Write-Host "Validating $BizTalkInstallFolder ..."
$cnt = 0;
while ((-not (Test-Path '$BizTalkInstallFolder\$application')) -and ($cnt -lt 3)) { Start-Sleep -seconds 5; $cnt = $cnt +1 }
if ($cnt -eq 3) { 
	$t = Invoke-Command -ScriptBlock { param($p1,$p2) Write-Host "$p1\$p2\"; Get-ChildItem "$p1" -ErrorAction:SilentlyContinue } -ArgumentList:$BizTalkInstallFolder,$application 
	if ($t -eq $null) {
		# Log failure of deployment
		if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);59;DeploySSO failed" }
		$errorMsg = "'$BizTalkInstallFolder' cannot be located"
		$Host.UI.WriteErrorLine($errorMsg)
		throw "'$BizTalkInstallFolder' cannot be located"
		exit
	}
	$t1 = $t | where {$_.Name -eq $application }
	if ($t1 -eq $null) {
		# Log failure of deployment
		if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);59;DeploySSO failed" }
		$errorMsg = "'$BizTalkInstallFolder\$application' cannot be located"
		$Host.UI.WriteErrorLine($errorMsg)
		throw "'$BizTalkInstallFolder\$application' cannot be located"
		exit
	}
}

# deploy the sso application in the group
if ($BizTalkDeploySSO -ne $null) {
	Write-Host "Invoke DeploySSO on" $appssoserver "..."
	# loop files in SSO folder
	$ssoFiles = Get-ChildItem "$BizTalkInstallFolder\$application\Resources\SSO" -ErrorAction:SilentlyContinue
	foreach ($ssoFile in $ssoFiles) {
	   $ssoApp = "$BizTalkInstallFolder\$application\Resources\SSO\" + $ssoFile
	   if ($deployLocal) {
		   DeploySSO -application:$application -ssoApp:$ssoApp -ErrorAction:SilentlyContinue -ErrorVariable:myError
	   }
	   else {
		   Invoke-Command -ComputerName:$appssoserver -FilePath:"$BizTalkPSFolder\DeploySSO.ps1" -ArgumentList:$application, $ssoApp -ConfigurationName:Microsoft.Powershell32 -Authentication:CredSSP -Credential:$cred -ErrorAction:SilentlyContinue -ErrorVariable:myError
	   }
	   if ($myError -ne $null) {
		   if ($myError.Count -gt 0) {
		   # Log failure of deployment
		   if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);59;DeploySSO failed $myError[0]" }
		   $Host.UI.WriteErrorLine($myError[0])
		   throw "DeploySSO on server $appssoserver failed for '$application'"
		   exit
		   }
	   }
	}
}

# Deploy Itinerary
if (!(Get-Alias -Name 'DeployItinerary' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'DeployItinerary' -Value "$BizTalkPSFolder\DeployItinerary.ps1"
}

# deploy the itineraries in the group
if ($BizTalkDeployItinerary -ne $null) {
	Write-Host "Invoke DeployItinerary on" $appserver "..."
	if ($BizTalkInstallFolder -eq $null) { $BizTalkInstallFolder = "\\$appserver\C$\Program Files (x86)\Generated by BizTalk" }
	# loop files in resources folder
	$itineraryFiles = Get-ChildItem -Path:"$BizTalkInstallFolder\$application\Resources" -Filter:"*itinerary*.xml" -ErrorAction:SilentlyContinue
	foreach ($itineraryFile in $itineraryFiles) {
	   $filetodeploy = "$BizTalkInstallFolder\$application\Resources\" + $itineraryFile
	   if ($deployLocal) {
		   DeployItinerary -application:$application -itinerary:$filetodeploy -ErrorAction:SilentlyContinue -ErrorVariable:myError
	   }
	   else {
		   Invoke-Command -ComputerName:$appserver -FilePath:"$BizTalkPSFolder\DeployItinerary.ps1" -ArgumentList:$application, $filetodeploy -ConfigurationName:Microsoft.Powershell32 -Authentication:CredSSP -Credential:$cred -ErrorAction:SilentlyContinue -ErrorVariable:myError
	   }
	   if ($myError -ne $null) {
		   if ($myError.Count -gt 0) {
		   # Log failure of deployment
		   if ($logfile -ne "") { Add-Content $logfile -value "$application;;$username;;$(Get-Date -format yyyyMMddhhmmss);59;DeployItinerary failed $myError[0]" }
		   $Host.UI.WriteErrorLine($myError[0])
		   throw "DeployItinerary on server $appserver failed for '$application'"
		   exit
		   }
	   }
	}
}

Set-Location $currentLoc
Write-Host "Deployment finished after" (Get-Date).subtract($startdate).TotalSeconds "seconds"

# Log end of deployment
if ($logfile -ne "") { Add-Content $logfile -value "$application;$msi;$username;$env;$(Get-Date -format yyyyMMddhhmmss);100;Install complete" }
