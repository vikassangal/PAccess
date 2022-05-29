
# Change the execution policy since scripts cannot execute by default as a security measure in PowerShell
set-executionpolicy remotesigned

# Import the RSA Keys from file
& $env:windir\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe -pi "PatientAccessKeys" "PatientAccessKeys.xml"

# Run ASP.NET applications using the NT Authority\Network Service account
& $env:windir\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis -pa "PatientAccessKeys" "NT AUTHORITY\NETWORK SERVICE"

# Grant access to the ASP.NET application identity
$hostname = $env:computername
echo $hostname
& $env:windir\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis -pa "PatientAccessKeys" "$hostname\ASPNET"


# Create a function to add a new ProtectionProvider called PatientAccessProtectedConfigurationProvider to the <providers> section of Machine.config
function Add-ConfigProvider ( $configFile ) 
{
	[xml]$xmlFile = get-content $configFile
	
	$existingNode = $xmlFile.SelectNodes('/configuration/configProtectedData/providers/add[@name="PatientAccessProtectedConfigurationProvider"]')	
	
	if ( $existingNode.Count -eq 0 )
	{

		$providers = $xmlFile.configuration.configProtectedData.providers
		
		$paProvider = $xmlFile.CreateElement("add")
		
		$attrName = $xmlFile.CreateAttribute("name")
		$attrName.psbase.Value = "PatientAccessProtectedConfigurationProvider"
		$paProvider.SetAttributeNode($attrName)
		
		$attrType = $xmlFile.CreateAttribute("type")
		$attrType.psbase.Value = "System.Configuration.RsaProtectedConfigurationProvider,System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		$paProvider.SetAttributeNode($attrType)
		
		$attrDescription = $xmlFile.CreateAttribute("description")
		$attrDescription.psbase.Value = "Uses RsaCryptoServiceProvider to encrypt and decrypt"
		$paProvider.SetAttributeNode($attrDescription)
		
		$attrKeyContainerName = $xmlFile.CreateAttribute("keyContainerName")
		$attrKeyContainerName.psbase.Value = "PatientAccessKeys"
		$paProvider.SetAttributeNode($attrKeyContainerName)
		
		$attrCspProviderName = $xmlFile.CreateAttribute("cspProviderName")
		$attrCspProviderName.psbase.Value = ""
		$paProvider.SetAttributeNode($attrCspProviderName)
		
		$attrUseMachineContainer = $xmlFile.CreateAttribute("useMachineContainer")
		$attrUseMachineContainer.psbase.Value = "true"
		$paProvider.SetAttributeNode($attrUseMachineContainer)
		
		$attrUseOAEP = $xmlFile.CreateAttribute("useOAEP")
		$attrUseOAEP.psbase.Value = "false"
		$paProvider.SetAttributeNode($attrUseOAEP)
		
		$providers.AppendChild($paProvider)
		
		$xmlFile.Save($configFile)
		
	}
}

# Modify Machine.config file
Add-ConfigProvider $env:windir\Microsoft.NET\Framework\v2.0.50727\CONFIG\machine.config

