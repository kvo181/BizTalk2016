param
(
    [string]$folder,
    [string]$username,
    [string]$permissions
)

Write-Host 'Creating folder ' $folder

if (!(Test-Path $folder)) {
New-Item $folder -type directory 

$colRights = [System.Security.AccessControl.FileSystemRights]$permissions

$InheritanceFlag = [System.Security.AccessControl.InheritanceFlags]'ContainerInherit,ObjectInherit' 
$PropagationFlag = [System.Security.AccessControl.PropagationFlags]::InheritOnly 

$objType =[System.Security.AccessControl.AccessControlType]::Allow 

$objUser = New-Object System.Security.Principal.NTAccount($username) 

$objACE = New-Object System.Security.AccessControl.FileSystemAccessRule `
    ($objUser, $colRights, $InheritanceFlag, $PropagationFlag, $objType) 

$objACL = Get-ACL $folder
$objACL.AddAccessRule($objACE) 

Set-ACL $folder $objACL
}    