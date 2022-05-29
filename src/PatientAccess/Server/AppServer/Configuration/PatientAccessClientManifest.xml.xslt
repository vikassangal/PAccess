<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date" xmlns:guid="urn:guid">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:param name="ManifestGuid" />
  <xsl:param name="Version">1.0.0.0</xsl:param>
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <manifest manifestId="{{{$ManifestGuid}}}" mandatory="False"
      xmlns="urn:schemas-microsoft-com:PAG:updater-application-block:v2:manifest">
      <description>Patient Access Client Application Manifest (<xsl:value-of select="$Branch" />-<xsl:value-of select="$Environment" />) version <xsl:value-of select="$Version" /> created on <xsl:value-of select="date:date-time()" />
      </description>
      <application applicationId="{/Settings/ServerSettings/AppUpdater/AppId[contains(@Environment,$Environment) and @Branch=$Branch]/text()}">
        <entryPoint file="PatientAccess.exe" parameters="" />
        <location></location>
      </application>
      <files base="{/Settings/ServerSettings/AppUpdater/ClientDownloadUrl[contains(@Environment,$Environment) and @Branch=$Branch]/text()}">
        <file source="PatientAccessClientWin32.zip" />
      </files>
      <activation>
        <tasks>
          <task name="ApplicationDeployProcessor" type="Microsoft.ApplicationBlocks.Updater.ActivationProcessors.ApplicationDeployProcessor,	Microsoft.ApplicationBlocks.Updater.ActivationProcessors" />
        </tasks>
      </activation>
    </manifest>
  </xsl:template>
</xsl:stylesheet>