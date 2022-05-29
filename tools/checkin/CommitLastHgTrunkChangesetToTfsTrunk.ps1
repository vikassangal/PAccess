#
# CommitLastHgTrunkChangesetToTfsTrunk.ps1
#

param(
		[switch]$Force,		
		$HgRevMatchingLatestTFSTrunkCommit,		
		$HgRevisionToCheckin
	  )

if(!$HgRevMatchingLatestTFSTrunkCommit)
{
    $HgRevMatchingLatestTFSTrunkCommit = "Trunk^"
}

if(!$HgRevisionToCheckin)
{
    $HgRevisionToCheckin = "Trunk"
}

$checkoutDirectory = Resolve-Path(Join-Path $pwd "..\..\")
$trunkTools = Join-Path $checkoutDirectory "tools\"
$trunkToolsCheckin = Join-Path $trunkTools "checkin\"
$scorchPath = Join-Path $trunkToolsCheckin "Scorch.bat"
$onlinePath = Join-Path $trunkToolsCheckin "Online.bat"

$pathTFVS2010 = "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"
$pathTFVS2012 = "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\tf.exe"
$pathTFVS2015 = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\tf.exe"

if(Test-Path $pathTFVS2010)
{
    $pathTF = $pathTFVS2010
}

elseif(Test-Path $pathTFVS2012)
{
    $pathTF = $pathTFVS2012
}

else
{
    $pathTF = $pathTFVS2015
}

function AreTherePendingChanges()
{	
	$statusMessage = & $pathTF status
	return ($statusMessage -ne "There are no pending changes.")
}

function UndoPendingChanges()
{	
	$undoChangesResult =  & $pathTF undo /recursive /noprompt .\
	return $undoChangesResult	
}

function RunScorch()
{
	cd $trunkToolsCheckin
	Write-Host "Running scorch...`n"
	$scorchMessage = & $scorchPath
	Write-Host "Finished running scorch!`n"
}

function HgRepoHasUncommitedChanges()
{
	$hgStatus = & hg status

	#$hgStatus == null means no changes
	if($hgStatus)
	{
		return $true	
	}
	else
	{
		return $false
	}
}

function RunCheckinTool()
{
    & $pathTF checkin
}

cls

cd $checkoutDirectory
$currentLocation = Get-Location
Write-Host "Current Directory: " $currentLocation "`n"

$areTherePendingChanges = AreTherePendingChanges

#1.	Close the solution in Visual Studio (VS)

#2.	Check for TFS pending changes and undo if forced
if($areTherePendingChanges)
{
	Write-Host "Pending changes found`n"
	
	if(!$Force)
	{
		Write-Host "Please commit your changes or undo pending changes and run the script again. `n"
		Exit
	}
	
	else
	{	
		Write-Host "Undoing pending changes...`n"
		$undoChangesResult = UndoPendingChanges
		Write-Host $undoChangesResult "`n"
		Write-Host "Finished undoing changes`n"	
	}	
}
	
else
{
	Write-Host "No TFS pending changes found.`n"
}

$hgRepoHasUncommitedChanges = HgRepoHasUncommitedChanges

if($hgRepoHasUncommitedChanges -and  !$Force)
{
	Write-Host "Uncommited Mercrial changes found please commit them or undo them and run the script again. `n"
	Exit
}

#3.	Get latest from the TFS trunk branch 
Write-Host "Getting latest from TFS!`n"

& $pathTF get /recursive /noprompt

Write-Host "`nFinished getting latest from TFS`n"

#4.	Run scorch.bat (located in “Trunk\tools\checkin\”) and scorch all files to ensure that the TFS check-out is in synch with the TFS server
RunScorch

#5.	In the Mercurial repository explorer update to the latest commit on the Trunk branch
Write-Host "Updating the mercurial repo to revision corresponging to the last commit on TFS Trunk`n"
write-host "a" | hg update -C $HgRevMatchingLatestTFSTrunkCommit

Write-Host "Updated to: "
hg id -i -n -b

#6.	In the Mercurial repository explorer view the commit dialog, there shouldn’t be anything to commit there, in case there is something to commit then it means that the TFS trunk and the Mercurial trunk are not in sync. They need to be synchronized before you can proceed. Once the trunk branches are synchronized start with step 1
cd $checkoutDirectory

Write-Host "Checking Hg status...`n"
Write-Host "Hg status:`n"
$hgStatus = & hg status

#$hgStatus == null means no changes
if($hgStatus)
{
	Write-Host "not in synch"	
}
else
{
	write-host "a" | hg update $HgRevisionToCheckin
	Write-Host "trunks are in synch"
	cd $trunkToolsCheckin
	Write-Host "Running online...`n"
	$onlineMessage = & $onlinePath
	$onlineMessage	| Write-Host
}

RunCheckinTool

Write-Host "Done!`n"

