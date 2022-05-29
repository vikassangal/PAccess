<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <configuration>
      <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
      <configSections>
        <section name="appStart" type="PatientAccess.AppStart.ConfigSectionHandler,PatientAccess.AppStart" />
        <section name="UpdaterConfiguration"  type="Microsoft.ApplicationBlocks.Updater.Configuration.ApplicationUpdaterSettings, Microsoft.ApplicationBlocks.Updater" />
        <section name="loggingConfiguration" type="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings, Microsoft.Practices.EnterpriseLibrary.Logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null" />
        <section name="securityCryptographyConfiguration" type="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CryptographySettings, Microsoft.Practices.EnterpriseLibrary.Security.Cryptography, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null" />
      </configSections>
      <appStart>
        <ClientApplicationInfo>
          <appFolderName>.\PatientAccess\</appFolderName>
          <appExeName>PatientAccess.exe</appExeName>
          <appID><xsl:value-of select="/Settings/ServerSettings/AppUpdater/AppId[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()"/></appID>
          <updateTime>BeforeStart</updateTime>
          <manifestUri><xsl:value-of select="/Settings/ClientSettings/AppStart/ManifestUrl[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()" /></manifestUri>
          <maxWaitTime>0</maxWaitTime>
          <appZipArchive>PatientAccessClientWin32.zip</appZipArchive>
        </ClientApplicationInfo>
      </appStart>
      <UpdaterConfiguration defaultDownloader="BITS" 
                            basePath="." 
                            applicationId="{/Settings/ServerSettings/AppUpdater/AppId[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" 
                            manifestUri="{/Settings/ClientSettings/AppStart/ManifestUrl[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}">
        <downloaders>
          <add name="BITS"
               type="Microsoft.ApplicationBlocks.Updater.Downloaders.BitsDownloader, Microsoft.ApplicationBlocks.Updater.Downloaders"
               userName=""
               password=""
               authenticationScheme="BG_AUTH_SCHEME_BASIC"
               targetServerType="BG_AUTH_TARGET_SERVER" />
        </downloaders>
        <service updateInterval="100000" />
        <manifestManager authenticationMethod="Anonymous" userName="" />
      </UpdaterConfiguration>
      <loggingConfiguration name="Logging Application Block" 
                            tracingEnabled="true" 
                            defaultCategory=""
                            logWarningsWhenNoCategoriesMatch="true">
        <listeners>
          <add fileName="trace.log"
               header="----------------------------------------"
               footer="----------------------------------------"
               formatter="Text Formatter"
               listenerDataType="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FlatFileTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"
               traceOutputOptions="None"
               type="Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.FlatFileTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"
               name="FlatFile TraceListener" />
        </listeners>
        <formatters>
          <add template="Timestamp: {{timestamp}}
           Message: {{message}}
           Category: {{category}}
           Priority: {{priority}}"
               type="Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.TextFormatter, Microsoft.Practices.EnterpriseLibrary.Logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"
               name="Text Formatter" />
        </formatters>
        <specialSources>
          <allEvents switchValue="All" 
                     name="All Events">
            <listeners>
              <add name="FlatFile TraceListener" />
            </listeners>
          </allEvents>
          <notProcessed switchValue="All" 
                        name="Unprocessed Category" />
          <errors switchValue="All" 
                  name="Logging Errors and Warnings" />
        </specialSources>
      </loggingConfiguration>
      <securityCryptographyConfiguration>
        <hashProviders>
          <add algorithmType="System.Security.Cryptography.MD5CryptoServiceProvider, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
               saltEnabled="true" 
               type="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.HashAlgorithmProvider, Microsoft.Practices.EnterpriseLibrary.Security.Cryptography, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"
               name="MD5CryptoServiceProvider" />
        </hashProviders>
      </securityCryptographyConfiguration>
      <system.net>
        <defaultProxy>
          <bypasslist>
            <add address="[a-z]+\.hdc\.net$" />
            <add address="[a-z]+\.mdltenethealth\.net$" />
            <add address="[a-z]+\.testtenethealth\.net$" />
            <add address="[a-z]+\.tenethealth\.net$" />
          </bypasslist>
        </defaultProxy>
      </system.net>
    </configuration>
  </xsl:template>
</xsl:stylesheet>