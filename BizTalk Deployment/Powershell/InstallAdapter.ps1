
param 
( 
    [string]$msi = "SomeApp.msi",
    [System.Management.Automation.PSCredential]$cred
)

Write-Host "Execute install with:"
Write-Host "   msi = $msi"
$startDate = Get-Date
Write-Host "On" $startDate

# validate the msi file
if (!(Test-Path -Path "$msi" -PathType Leaf)) {
    Write-Host "$msi could not be located!"
    exit
}

$credential = $cred.GetNetworkCredential()

#Install an adapter on the computer
Write-Host "Installing" $msi "with user" $credential.UserName "of domain" $credential.Domain "..." 

$parameters = "/qn /i " + $msi
$installStatement = [System.Diagnostics.Process]::Start( "msiexec", $parameters) 
#$installStatement = [System.Diagnostics.Process]::Start( "msiexec", $parameters, $credential.UserName, $cred.Password, $credential.Domain) 
$installStatement.WaitForExit()
$exitcode = $installStatement.ExitCode
Write-Host "ExitCode=" $exitCode
if ($exitcode -ne 0) { throw "Error installing $msi" }
$endDate = Get-Date
Write-Host "Install finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
Write-Host ""
