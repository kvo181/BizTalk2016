<#
Created by: Jeroen Hendriks
E-mail: Jeroen@BizTalkAdminsBlogging.com
Date: 29 Februari 2011	
Version: 2.0
Stop or start BizTalk host instances and IIS on a BizTalk server

Modified by: Koen Van Oost
Date: 27/07/2012
Enable it to be called from our deployment tool
Enable Start/Stop IIS seperately
#>

param 
( 
    [Parameter(Position=0, Mandatory=$true, HelpMessage="SQL Service instance")] 
	[ValidateNotNullOrEmpty()]
    [string]$server, 
    [Parameter(Position=1, Mandatory=$true, HelpMessage="BizTalk Management Database e.g.: BizTalkMgmtDb")] 
	[ValidateNotNullOrEmpty()]
    [string]$database,
    [Parameter(Position=2, Mandatory=$true, HelpMessage="BizTalk environment LOC, DEV, ...")] 
    [string]$env,
    [Parameter(Mandatory=$false, HelpMessage="Log info into a log file")] 
	[string]$logfile,
    [Parameter(Mandatory=$false, HelpMessage="Turn debugging On or Off")] 
    [string]$debugScript
)

#Set the text for the option All BizTalk Servers
$AllBizTalkServers = "All BizTalk Servers"

#Clear variable cancel
#clear-variable -name cancel

### Functions ###
Function GetUserInput
{    
    [void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null
    [void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") | Out-Null

    $objForm = New-Object System.Windows.Forms.Form 
    $objForm.Text = "Stop or start BizTalk servers"
    $objForm.Size = New-Object System.Drawing.Size(1000,385)
    $objForm.maximumsize =new-object drawing.size @(1000,385)
    $objForm.minimumsize =new-object drawing.size @(1000,385)
    $objForm.maximizebox = $false 
    $objForm.StartPosition = "CenterScreen"

    $objForm.KeyPreview = $True
    $objForm.Add_KeyDown({if ($_.KeyCode -eq "Enter") 
        {$script:ServerToStopOrStart = $objform.SelectedItem;$objForm.Close()}})
    $objForm.Add_KeyDown({if ($_.KeyCode -eq "Escape") 
        {$objForm.Close()}})

    #Create Execute Button
    $ExecuteButton = New-Object System.Windows.Forms.Button
    $ExecuteButton.Location = New-Object System.Drawing.Size(310,40)
    $ExecuteButton.Size = New-Object System.Drawing.Size(125,23)
    $ExecuteButton.Text = "Execute"
    $ExecuteButton.Add_Click({PressExecute})
    $objForm.Controls.Add($ExecuteButton)

    #Create Exit Button
    $ExitButton = New-Object System.Windows.Forms.Button
    $ExitButton.Location = New-Object System.Drawing.Size(440,40)
    $ExitButton.Size = New-Object System.Drawing.Size(125,23)
    $ExitButton.Text = "Exit"
    $ExitButton.Add_Click({$objForm.Close(); $cancel = $true})
    $objForm.Controls.Add($ExitButton)
    
    #Create Refresh Button
    $RefreshButton = New-Object System.Windows.Forms.Button
    $RefreshButton.Location = New-Object System.Drawing.Size(310,70)
    $RefreshButton.Size = New-Object System.Drawing.Size(125,23)
    $RefreshButton.Text = "Refresh"
    $RefreshButton.Add_Click({PressRefresh})
    $objForm.Controls.Add($RefreshButton)

    #Create Clear output Button
    $RefreshButton = New-Object System.Windows.Forms.Button
    $RefreshButton.Location = New-Object System.Drawing.Size(440,70)
    $RefreshButton.Size = New-Object System.Drawing.Size(125,23)
    $RefreshButton.Text = "Clear output"
    $RefreshButton.Add_Click({Clearoutput})
    $objForm.Controls.Add($RefreshButton)

    #Create a label
    $objLabel = New-Object System.Windows.Forms.Label
    $objLabel.Location = New-Object System.Drawing.Size(10,130) 
    $objLabel.Size = New-Object System.Drawing.Size(280,20) 
    $objLabel.Text = "Please select a BizTalk server:"
    $objForm.Controls.Add($objLabel) 
    
    #Create groupbox
    $groupbox = New-Object System.Windows.Forms.groupbox 
    $groupbox.Location = New-Object System.Drawing.Size(10,145) 
    $groupbox.Size = New-Object System.Drawing.Size(260,20) 
    $groupbox.Height = 180
    $objForm.Controls.Add($groupbox) 
    
    #Add $AllBizTalkServers radiobutton
    $radiobuttonstart = New-Object Windows.Forms.radiobutton
    $radiobuttonstart.text = $AllBizTalkServers
    $radiobuttonstart.Location = New-Object System.Drawing.Point(5,15)  
    $radiobuttonstart.Size = new-object System.Drawing.Size(250,15) 
    $radiobuttonstart.checked = "True"
    $groupbox.controls.add($RadioButtonstart)
    
    #Add radiobuttons for other servers    
    $radiobuttonTop = 15
    $radiobuttonTopIncrement = 20
    foreach ($item_server in $servers)
        {
        # create radiobuttonstart
        $radiobuttonTop = $radiobuttonTop + $radiobuttonTopIncrement
        $radiobuttonstart = New-Object Windows.Forms.radiobutton
        $radiobuttonstart.text = $item_server
        $radiobuttonstart.Location = New-Object System.Drawing.Point(5,$radiobuttonTop)  
        $radiobuttonstart.Size = new-object System.Drawing.Size(250,15) 
        $groupbox.controls.add($RadioButtonstart)
        }
    
    #Create a label
    $objLabelAction = New-Object System.Windows.Forms.Label
    $objLabelAction.Location = New-Object System.Drawing.Size(10,20) 
    $objLabelAction.Size = New-Object System.Drawing.Size(280,20) 
    $objLabelAction.Text = "Do you want to stop or start?"
    $objForm.Controls.Add($objLabelAction) 
    
    #Create groupbox
    $groupboxAction = New-Object System.Windows.Forms.groupbox 
    $groupboxAction.Location = New-Object System.Drawing.Size(10,35) 
    $groupboxAction.Size = New-Object System.Drawing.Size(260,20) 
    $groupboxAction.Height = 80
    $objForm.Controls.Add($groupboxAction) 
    
    #Add start radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Start BizTalk server(s)"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(5,15)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(150,15)
    $radiobuttonAction.checked = "True" 
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Add stop radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Stop BizTalk server(s)"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(5,35)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(150,15) 
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Add Restart radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Restart BizTalk server(s)"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(5,55)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(150,15) 
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Add start (IIS) radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Start IIS"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(155,15)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(75,15)
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Add stop (IIS) radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Stop IIS"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(155,35)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(75,15) 
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Add Restart (IIS) radiobutton
    $radiobuttonAction = New-Object Windows.Forms.radiobutton
    $radiobuttonAction.text = "Restart IIS"
    $radiobuttonAction.Location = New-Object System.Drawing.Point(155,55)  
    $radiobuttonAction.Size = new-object System.Drawing.Size(95,15) 
    $groupboxAction.controls.add($radiobuttonAction)   
    
    #Create a label
    $objLabel = New-Object System.Windows.Forms.Label
    $objLabel.Location = New-Object System.Drawing.Size(310,130) 
    $objLabel.Size = New-Object System.Drawing.Size(280,20) 
    $objLabel.Text = "BizTalk status"
    $objForm.Controls.Add($objLabel) 
    
    #Create a status box 
    $objlistBox = New-Object System.Windows.Forms.listBox
    $objlistBox.text = $servers
    $objlistBox.Location = New-Object System.Drawing.Size(310,150)
    $objlistBox.Size = New-Object System.Drawing.Size(260,185)
    $objForm.Controls.Add($objlistBox) 
    
    #Create initial status output
    GetStatus  
     
    #Create a label
    $objLabel = New-Object System.Windows.Forms.Label
    $objLabel.Location = New-Object System.Drawing.Size(610,20) 
    $objLabel.Size = New-Object System.Drawing.Size(280,20) 
    $objLabel.Text = "Output"
    $objForm.Controls.Add($objLabel) 
    
    #Create a output box 
    $objlistBoxOutput = New-Object System.Windows.Forms.listBox
    $objlistBoxOutput.Location = New-Object System.Drawing.Size(610,40)
    $objlistBoxOutput.Size = New-Object System.Drawing.Size(360,290)
    $objForm.Controls.Add($objlistBoxOutput) 

    $objForm.Add_Shown({$objForm.Activate()})
    [void] $objForm.ShowDialog()
    
    #If Exit get's pressed, the script exists.
    if ($cancel -eq "True") {exit}
}

#This function fills hostinstances on the server defined in $server in the array
function HostInstances ($server)
{
	if ($localserver -eq $appserver) {
    $script:HostInstances = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"} | select-object -unique -expand hostname
	}
	else {
    $script:HostInstances = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' -Impersonation 4 -ComputerName $appserver | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"} | select-object -unique -expand hostname
	}
}

#Get all BizTalk servers in the BizTalk group
Function getservers
{
	if ($localserver -eq $appserver) {
    $script:servers = get-wmiobject MSBTS_Server -namespace root\MicrosoftBizTalkServer | select-object -expand name
	}
	else {
    $script:servers = get-wmiobject MSBTS_Server -namespace root\MicrosoftBizTalkServer -Impersonation 4 -ComputerName $appserver | select-object -expand name
	}
}
#Stop host instances
Function StopHostInstance ($server, $hostinstance)
{
    If ($debugScript -eq "On") {write-host "Stopping $hostinstance on $server"}
	if ($localserver -eq $appserver) {
    $StopHostInstance = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True" -and $_.hostname -eq $HostInstance}
	}
	else {
    $StopHostInstance = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' -Impersonation 4 -ComputerName $appserver | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True" -and $_.hostname -eq $HostInstance}
	}
    write-host $StopHostInstance
    $stopHostInstance.stop() | out-null
}

Function GetStatus
{
    #Clear all items
    $objListBox.Items.clear()
    foreach ($server in $servers)
        {
        #Get all host instances and the number of host instances
	if ($localserver -eq $appserver) {
        $HostInstancesCount = (get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"}).count
        $HostInstances = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"}
	}
	else {
        $HostInstancesCount = (get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' -Impersonation 4 -ComputerName $appserver | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"}).count
        $HostInstances = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' -Impersonation 4 -ComputerName $appserver | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True"}
	}
        
        #Reset values
        $NumberOfHostInstancesStopped = 0
        $NumberOfHostInstancesStarted = 0 

        #Count how many host instances are stopped or started
        foreach ($HostInstance in $HostInstances)
            {
            if ($HostInstance.servicestate -eq "1")
                {
                If ($debugScript -eq "On") {write-host $HostInstance.hostname" is stopped"}
                $NumberOfHostInstancesStopped ++
                }
            if ($HostInstance.servicestate -eq "4")
                {
                If ($debugScript -eq "On") {write-host $HostInstance.hostname" is started"}
                $NumberOfHostInstancesStarted ++
                }
            }

            #if all host instances are stopped, give stopped status.
            if ($NumberOfHostInstancesStopped -eq $HostInstancesCount)
                {
                If ($debugScript -eq "On") {write-host "$server is stopped."}
                #Create status output
                $objListBox.Items.Add("$server is stopped.")
                }
            #if all host instances are started, give started status.
            if ($NumberOfHostInstancesStopped -eq 0)
                {
                If ($debugScript -eq "On") {write-host "$server is started."}
                #Create status output
                $objListBox.Items.Add("$server is started.")
                }
            #if some host instances are stopped and others are started, give partially started status.
            if ($NumberOfHostInstancesStopped -gt 0 -AND $NumberOfHostInstancesStopped -lt $HostInstancesCount)
                {
                If ($debugScript -eq "On") {write-host "server is partially started"}
                #Create status output
                $objListBox.Items.Add("$server is partially started.")
                } 
             }    
}

Function PressRefresh
{
   GetStatus ($server)
   If ($debugScript -eq "On") {Write-host "Refresh button clicked"} 
}

#Stop start instances
Function StartHostInstance ($server, $hostinstance)
{
	if ($localserver -eq $appserver) {
    $StartHostInstance = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True" -and $_.hostname -eq $HostInstance}
	}
	else {
    $StartHostInstance = get-wmiobject MSBTS_HostInstance -namespace 'root\MicrosoftBizTalkServer' -Impersonation 4 -ComputerName $appserver | where {$_.runningserver -eq $server -AND $_.hosttype -ne "2" -and $_.IsDisabled -ne "True" -and $_.hostname -eq $HostInstance}
	}
    $StartHostInstance.start() | out-null
}

Function Clearoutput
{
    $script:i = 0
    $objlistBoxOutput.Items.clear()
}

#Remove items from the outputbox if there are more then 20 items in it. 
Function RemoveExcesItemsOutputbox
{
    if ($objlistBoxOutput.Items.count -gt 20) {write-host $objlistBoxOutput.Items.RemoveAt(0)}
}

#Actions performed when the OK button gets pressed
Function PressExecute
{   
    #Select action (stop or start) 
    $script:Action = ($groupboxAction.Controls | where-object {$_.checked -eq "True"} | select-object -expand text)
    #Set action name for debugging purpopses.
    if ($action -eq "Stop BizTalk server(s)") {$script:actionname = "stopped"} 
    if ($action -eq "Start BizTalk server(s)") {$script:actionname = "started"} 
    if ($action -eq "Stop IIS") {$script:actionname = "IIS stopped"} 
    if ($action -eq "Start IIS") {$script:actionname = "IIS started"} 
    
    #Select the servers that have to be started or stopped.
    $script:ServersToStopOrStart = ($groupbox.Controls | where-object {$_.checked -eq "True"} | select-object -expand text)
    if ($ServersToStopOrStart -eq $AllBizTalkServers) {$ServersToStopOrStart = $servers}
    
    foreach ($ServerToStopOrStart in $ServersToStopOrStart)
    {        
        #Get the host instances on the server that will be stopped or started.
        HostInstances ($ServerToStopOrStart)
        If ($debugScript -eq "On") {write-host "The following host instances on server $ServerToStopOrStart will be $actionname."}
        If ($debugScript -eq "On") {foreach ($HostInstance in $HostInstances) {write-host $HostInstance}}
        
        #Stop or start the host instances
        foreach ($HostInstance in $HostInstances)
        {            
            RemoveExcesItemsOutputbox
            if ($action -eq "Stop BizTalk server(s)") 
                {
                StopHostInstance $ServerToStopOrStart $hostinstance
                $script:i ++
                $objlistBoxOutput.Items.Add("$i Stopped $hostinstance on $ServerToStopOrStart")
				if ($logfile -ne "") { 
					Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);3;$i Stopped $hostinstance on $ServerToStopOrStart" 
				}
                }
            if ($action -eq "Start BizTalk server(s)") 
                {
                StartHostInstance $ServerToStopOrStart $hostinstance
                $script:i ++
                $objlistBoxOutput.Items.Add("$i Started $hostinstance on $ServerToStopOrStart")
				if ($logfile -ne "") { 
					Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);2;$i Started $hostinstance on $ServerToStopOrStart" 
				}
                } 
            if ($action -eq "Restart BizTalk server(s)") 
                {
                StopHostInstance $ServerToStopOrStart $hostinstance
                StartHostInstance $ServerToStopOrStart $hostinstance
                $script:i ++
                $objlistBoxOutput.Items.Add("$i Restarted $hostinstance on $ServerToStopOrStart")
				if ($logfile -ne "") { 
					Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);4;$i Restarted $hostinstance on $ServerToStopOrStart" 
				}
                }                   
		}
        
        #Stop, start or restart IIS
            if ($action -eq "Stop IIS") 
                {
                StopIIS $ServerToStopOrStart
                }
            if ($action -eq "Start IIS") 
                {
                StartIIS $ServerToStopOrStart
                } 
            if ($action -eq "Restart IIS") 
                {
                StopIIS $ServerToStopOrStart
                StartIIS $ServerToStopOrStart
                }
            RemoveExcesItemsOutputbox              
    }

	$script:i ++
	$objlistBoxOutput.Items.Add("$i $action done!")
	
	
    #Refresh status
    GetStatus
}

Function StopIIS ($server)
{
	#It will stop the application pools who's name begins with 'BizTalk'
	
    #Get services
    $W3SVCService = Get-WmiObject Win32_Service -ComputerName $server -Filter "Name='W3SVC'"
    #if the service exist, continue.
    if ($W3SVCService -eq $null) { break }

    $pools = Get-WMIObject IISApplicationPool -ComputerName $server -Namespace root\MicrosoftIISv2 -Authentication PacketPrivacy | Where-Object { $_.Name -Like "W3SVC/APPPOOLS/BizTalk*" }
	foreach ($pool in $pools) { 
	    Invoke-WMIMethod Stop -Path $pool.__RELPATH -Computer $server -Namespace root\MicrosoftIISv2 -Authentication Packetprivacy
        $script:i ++
		$m = "$i Stopped AppPool (" + $pool.Name + ") on $ServerToStopOrStart"
	    $objlistBoxOutput.Items.Add($m)         
		if ($logfile -ne "") { 
			Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);3;$m" 
		}
	}
}

Function StartIIS ($server)
{
    #Get services
    $W3SVCService = Get-WmiObject Win32_Service -ComputerName $server -Filter "Name='W3SVC'"
    #if the service exist, continue.
    if ($W3SVCService -eq $null) { break }

    $pools = Get-WMIObject IISApplicationPool -ComputerName $server -Namespace root\MicrosoftIISv2 -Authentication PacketPrivacy | Where-Object { $_.Name -Like "W3SVC/APPPOOLS/BizTalk*" }
	foreach ($pool in $pools) { 
	    Invoke-WMIMethod Start -Path $pool.__RELPATH -Computer $server -Namespace root\MicrosoftIISv2 -Authentication PacketPrivacy
        $script:i ++
		$m = "$i Started AppPool (" + $pool.Name + ") on $ServerToStopOrStart"
	    $objlistBoxOutput.Items.Add($m)         
		if ($logfile -ne "") { 
			Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);2;$m" 
		}
	}
}

### End functions ###

### Main ###

$startLogDate = Get-Date -format yyyyMMddhhmmss -ErrorAction:Stop
$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
$localserver = $env:computername.ToLower() + ".$BizTalkDomain"

if ($logfile -ne "") { 
    New-Item $logfile -type file -force
    Add-Content $logfile -value ";;$username;$env;$startLogDate;1;Start Recycle BizTalk Servers" 
}

# GetServersInGroup
if (!(Get-Alias -Name 'GetServersInGroup' -ErrorAction:SilentlyContinue)) {
    Set-Alias -Name 'GetServersInGroup' -Value "$BizTalkPSFolder\GetServersInGroup.ps1"
}
$servers = GetServersInGroup -server $server -database $database
if ($servers.GetType().Name -eq "String") {
$appserver = $servers.ToLower() + ".$BizTalkDomain"
} else {
$appserver = $servers[0].ToLower() + ".$BizTalkDomain" }

#Get all the BizTalk servers in the BizTalk group
getservers
If ($debugScript -eq "On") {write-host "The following servers are in the BizTalk group:"}
If ($debugScript -eq "On") {foreach ($server in $servers) {write-host "server: $server"}}

#Select which BizTalk to start or stop
GetUserInput

if ($logfile -ne "") { 
	Add-Content $logfile -value ";;$username;$env;$(Get-Date -format yyyyMMddhhmmss);5;End Recycle BizTalk Servers" 
}

### End main ###