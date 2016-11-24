param
(
    [string]$folder
)

Write-Host 'Creating folder: ' $folder

if (!(Test-Path $folder)) {
    New-Item $folder -type directory }
    