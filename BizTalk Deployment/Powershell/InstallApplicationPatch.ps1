
param 
( 
    [string]$application = "SomeApp",
    [string]$msi = "SomeApp.msi"
)

Write-Host "Execute install patch with:"
Write-Host "   application = $application"
Write-Host "   msi = $msi"
$startDate = Get-Date
Write-Host "On" $startDate

# validate the msi file
if (!(Test-Path -Path "$msi" -PathType Leaf)) {
    Write-Host "$msi could not be located!"
    exit
}

#Install the application patch on the computer
Write-Host "Installing" $msi "..."

$parameters = "/qn /i " + $msi
$installStatement = [System.Diagnostics.Process]::Start( "msiexec", $parameters ) 
$installStatement.WaitForExit()
$exitcode = $installStatement.ExitCode
Write-Host "ExitCode=" $exitCode
#if ($exitcode -ne 0) { throw "Error installing $msi" }
$endDate = Get-Date
Write-Host "Install finished on" $endDate "and took" $endDate.Subtract($startDate).TotalSeconds "seconds"
Write-Host ""
