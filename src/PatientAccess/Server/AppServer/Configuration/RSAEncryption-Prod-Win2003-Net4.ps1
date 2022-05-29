# Import the RSA Keys from file to a key container
& $env:windir\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis -pi "PatientAccessKeys" "PatientAccessKeys.xml"

# Grant read-only access for the key container to the account used by the IIS app pool
& $env:windir\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis -pa "PatientAccessKeys" "TENETHEALTH\C90.PasService"

# Create a function to add a new ProtectionProvider called PatientAccessProtectedConfigurationProvider to the <providers> section of Machine.config
function Add-ConfigProvider ( $configFile ) 
{
	$xmlFile = [xml]([System.IO.File]::ReadAllText($configFile))
	
	$existingNode = $xmlFile.SelectNodes('/configuration/configProtectedData/providers/add[@name="PatientAccessProtectedConfigurationProvider"]')	
	
	if ( $existingNode.Count -eq 0 )
	{

		$providers = $xmlFile.configuration.configProtectedData.providers
		
		$paProvider = $xmlFile.CreateElement("add")
		
		$attrName = $xmlFile.CreateAttribute("name")
		$attrName.psbase.Value = "PatientAccessProtectedConfigurationProvider"
		$paProvider.SetAttributeNode($attrName)
		
		$attrType = $xmlFile.CreateAttribute("type")
		$attrType.psbase.Value = "System.Configuration.RsaProtectedConfigurationProvider,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"

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

# Modify the machine.config file
Add-ConfigProvider $env:windir\Microsoft.NET\Framework\v4.0.30319\CONFIG\machine.config
