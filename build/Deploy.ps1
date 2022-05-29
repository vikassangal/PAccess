# Make a list for Y/N choices
$noChoiceDescription =	New-Object System.Management.Automation.Host.ChoiceDescription("&No","Don't do it")
$yesChoiceDescription = New-Object System.Management.Automation.Host.ChoiceDescription("&Yes","Yeah, go ahead")
$yesNoChoices = [Management.Automation.Host.ChoiceDescription[]]($noChoiceDescription,$yesChoiceDescription)

# Make the choice descriptions for all available environments
$developmentEnvironemnt = New-Object System.Management.Automation.Host.ChoiceDescription("&Dev","Deploy to Development Servers")
$testEnvironment = New-Object System.Management.Automation.Host.ChoiceDescription("&Test","Deploy to Test Servers")
$modelEnvironment = New-Object System.Management.Automation.Host.ChoiceDescription("&Model","Deploy to Model Servers")
$betaEnvironment = New-Object System.Management.Automation.Host.ChoiceDescription("&Beta","Deploy to Beta Servers")
$prodEnvironment = New-Object System.Management.Automation.Host.ChoiceDescription("&Prod","Deploy to Production Servers")

# Make the choice descriptions for the branches
$trunkBranch = New-Object System.Management.Automation.Host.ChoiceDescription "&Trunk","Use the Trunk"
$supportBranch = New-Object System.Management.Automation.Host.ChoiceDescription "&Support","Use the Support Branch"
$branches = [Management.Automation.Host.ChoiceDescription[]]($trunkBranch,$supportBranch)

# Make the combinations of branch/environments
$supportEnvironments = [Management.Automation.Host.ChoiceDescription[]]($developmentEnvironemnt,$testEnvironment,$modelEnvironment,$betaEnvironment,$prodEnvironment)
$trunkEnvironments = [Management.Automation.Host.ChoiceDescription[]]($developmentEnvironemnt,$testEnvironment)
$branchEnvironments = ($trunkEnvironments,$supportEnvironments)

# Clean up the old source and target copies
function CleanSourceAndTarget([string]$sourceDirectory, [string]$targetDirectory)
{

	$maxOldCopies = 3

	$sourceFolders = Get-Item "$sourceDirectory\*" | Sort-Object -Property CreationTime	-Descending

	# Remove all but the newest three source folders
	if( $sourceFolders.Length -gt $maxOldCopies ) 
	{		
		$sourceDeletes = $sourceFolders | Select-Object -Last $($sourceFolders.Length - $maxOldCopies)
		$sourceDeletes | Get-ChildItem -Recurse | Remove-Item -Force -Recurse
		$sourceDeletes | Remove-Item -Force
	}	
	
	$targetFolders = Get-Item "$targetDirectory\*" | Sort-Object -Property CreationTime -Descending
	
	# Remove all but the newest three target folders
	if( $targetFolders.Length -gt $maxOldCopies ) 
	{
		$targetDeletes = $targetFolders | Select-Object -Last $($targetFolders.Length - $maxOldCopies)
		$targetDeletes | Get-ChildItem -Recurse | Remove-Item -Force -Recurse
		$targetDeletes | Remove-Item -Force
	}	

}

Clear-Host

# Ask the user to chooce branch, source, target, and version
$branchChoice = 
	$host.ui.PromptForChoice( "Step One:", "Choose the source-code branch from which you want to deploy:", $branches,  0 )
$sourceChoice = 
	$host.ui.PromptForChoice( "Step Two:", "Choose the *source* environment from which you want to deploy:", $branchEnvironments[$branchChoice],  0 )
$targetChoice = 
	$host.ui.PromptForChoice( "Step Three:", "Choose the *target* environment to which you want to deploy (can be the same as the source):", $branchEnvironments[$branchChoice],  0 )

# Build a list of available versions based on what is sitting on the filesystem
[System.IO.DirectoryInfo[]]$availableVersions = Get-Item ".\Builds\$($branches[$branchChoice].Label.substring(1))\$($branchEnvironments[$branchChoice][$sourceChoice].Label.substring(1))\*" | Sort-Object -Property CreationTime -Descending
# Stop if there isn't anything to deploy
if(-not $availableVersions)
{
	Write-Host "There are no builds available in the source directory"
	exit
}
[Management.Automation.Host.ChoiceDescription[]]$versionChoices = @()
$choiceNumber = 0
foreach($versionItem in $availableVersions)
{
	$choiceNumber++
	$versionChoices += New-Object System.Management.Automation.Host.ChoiceDescription "&$($choiceNumber) - $($versionItem.Name)","Deploy version $($versionItem.Name)" 
}

# Now use it to figure out which one we are going to deploy
$versionChoice = $host.ui.PromptForChoice( "Step Four:", "Choose the version you want to deploy:", $versionChoices,  0 )

# Confirm the choices
$deploySettings = @{"Branch" = $branches[$branchChoice].label.substring(1);
					"SourceEnvironment" = $branchEnvironments[$branchChoice][$sourceChoice].label.substring(1);
					"TargetEnvironment" = $branchEnvironments[$branchChoice][$targetChoice].label.substring(1);
					"VersionToDeploy" = $versionChoices[$versionChoice].label.substring(5) }
$deploySettings | Format-Table
$goChoice = $host.ui.PromptForChoice( "Final Step:", "Does everything look right?", $yesNoChoices,  0 )

# Do the actual work now that we have the information
if($goChoice -ne 0) 
{ 
	$sourceDirectory = ".\Builds\$($deploySettings['Branch'])\$($deploySettings['SourceEnvironment'])"
	$targetDirectory = ".\Builds\$($deploySettings['Branch'])\$($deploySettings['TargetEnvironment'])"

	# Move the files
	if($deploySettings["SourceEnvironment"] -ne $deploySettings["TargetEnvironment"])
	{
	
		Write-Host "Moving deployment files to target environment"
	
		Move-Item $availableVersions[$versionChoice] $targetDirectory
		
		# Update the environment setting
		$settingLocation = "$targetDirectory\$($deploySettings['VersionToDeploy'])\build\ContinuousIntegrationSettings.proj"
		$settingsXml = New-Object XML
		$settingsXml.Load($settingLocation)
		$settingsXml.Project.PropertyGroup.Environment = $deploySettings["TargetEnvironment"]
		$settingsXml.Save($settingLocation)
		
	}
	# ... or re-deploy
	else
	{
		Write-Host "Re-deploying to target environment"
	}
	
	# Launch MSBUILD to handle the rest of this
	C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:Configure`;CreateInstallationFiles`;Deploy;DeploymentNotification /p:GenerationOption=All "$targetDirectory\$($deploySettings['VersionToDeploy'])\build\ContinuousIntegration.proj"	| Tee-Object -FilePath "$targetDirectory\$($deploySettings['VersionToDeploy'])\Deployment.log"

	# Clean up old folders
	CleanSourceAndTarget $sourceDirectory $targetDirectory
	
	Write-Host "Deployment is complete."
	
}
else
{
	Write-Host "Deployment aborted"
}
