<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <configuration>
      <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
      <configSections>
        <!-- Register a section handler for the configuration section -->
        <section name="patientAccess" type="PatientAccess.ConfigSectionHandler,PatientAccess" />
        <!-- Register a section handler for the log4net section -->
        <section name="log4net" type="System.Configuration.IgnoreSectionHandler" />
      </configSections>
      <patientAccess>
        <ClientApplicationInfo>
          <appName>Patient Access</appName>
          <appFolderName>.\PatientAccess\</appFolderName>
          <appExeName>PatientAccess.exe</appExeName>
          <appID><xsl:value-of select="/Settings/ServerSettings/AppUpdater/AppId[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()"/></appID>
        </ClientApplicationInfo>
      </patientAccess>
      <system.net>
        <!-- Specify the maximum number of concurrent outbound calls to any network host; ie. '*'. -->
        <connectionManagement>
          <add address="*" maxconnection="12" />
        </connectionManagement>
        <!-- Ensure the calls to the AppServer do not got through a proxy server -->
        <defaultProxy>
          <bypasslist>
            <add address="[a-z]+\.hdc\.net$" />
            <add address="[a-z]+\.mdltenethealth\.net$" />
            <add address="[a-z]+\.tenethealth\.net$" />
          </bypasslist>
        </defaultProxy>
      </system.net>
      <!-- log4net Configuration -->
      <log4net>
        <!-- Setup the root category, add the appenders and set the default level -->
        <root>
          <level value="ERROR" />
          <appender-ref ref="AppServerAppender" />
        </root>
        <!-- Don't rename this logger without updating the named reference in PatientAccess.UI.CrashReporting.CrashReporter.cs -->
        <logger name="PatientAccess.UI.Logging.BreadCrumbLogger" additivity="false">
          <level value="INFO" />
          <appender-ref ref="BreadCrumbAppender" />
        </logger>
        <logger name="PatientAccess.UI.CommonControls.Wizard.WizardLinksControl" additivity="false">
          <level value="INFO" />
          <appender-ref ref="GenericAppender" />
        </logger>
        <appender name="AppServerAppender" type="PatientAccess.UI.Logging.AppServerAppender, PatientAccess">
          <!-- The number of events to buffer before sending -->
          <bufferSize value="1" />
          <mapping>
            <level value="ERROR" />
          </mapping>
        </appender>
        <appender name="GenericAppender" type="log4net.Appender.RollingFileAppender">
          <file value="GenericLog.txt" />
          <appendToFile value="true" />
          <datePattern value="yyyyMMdd" />
          <rollingStyle value="Size" />
          <maxSizeRollBackups value="1" />
          <maximumFileSize value="5MB" />
          <staticLogFileName value="true" />
          <filter type="log4net.Filter.LevelRangeFilter">
            <acceptOnMatch value="true" />
            <!-- Valid error levels (in order) are: ALL, DEBUG, INFO, WARN, ERROR, FATAL -->
            <levelMin value="INFO" />
            <levelMax value="FATAL" />
          </filter>
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%-5p %d %5rms %-22.22c{{1}} %-18.18M - %m%n" />
          </layout>
        </appender>
        <!-- Don't rename this appender without updating the named reference in PatientAccess.UI.CrashReporting.CrashReporter.cs -->
        <appender name="BreadCrumbAppender" type="log4net.Appender.RollingFileAppender">
          <lockingModel type="log4net.Appender.FileAppender+MinimalLock" />
          <file value="BreadCrumbLog.txt" />
          <appendToFile value="true" />
          <datePattern value="yyyyMMdd" />
          <rollingStyle value="Size" />
          <maxSizeRollBackups value="1" />
          <maximumFileSize value="2MB" />
          <staticLogFileName value="true" />
          <filter type="log4net.Filter.LevelRangeFilter">
            <acceptOnMatch value="true" />
            <!-- Valid error levels (in order) are: ALL, DEBUG, INFO, WARN, ERROR, FATAL -->
            <levelMin value="INFO" />
            <levelMax value="FATAL" />
          </filter>
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="PID:%-6property{{NDC}} ThreadID:%thread %-5p %d - %m%n" />
          </layout>
        </appender>
      </log4net>
      <appSettings>
        <add key="CCMaxServicePointIdleTime" value="3000" />
        <add key="CCCheckForCrossThreads" value="true" />
        <add key="CCAnnouncementsAfterCrash" value="true" />
        <add key="CCUseRemotingCompression" value="false" />
        <add key="CCRemotingErrorRetries" value="3" />
        <add key="CCRemotingTimeout" value="-1" />
        <add key="CCApplicationInActivityTimeout" value="900000" />
        <!-- date to start using MSP2 wizard -->
        <add key="MSP2_START_DATE" value="11/01/2006" />
        <!-- Determine if Crash Reporting test feature is enabled from the About Screen -->
        <add key="PatientAccess.EnableTestMode" value="False" />
        <!-- Determine if Broker Factory should use the remoting interface -->
        <add key="PatientAccess.IsServer" value="False" />
        <!-- Determine if Facility should use the NoMedicarePrimaryPayorForAutoAccident Rule -->
        <add key="UseMedicareCannotBePrimaryPayorForAutoAccidentRule" value="true" />
        <!-- Application Server URL -->
        <add key="PatientAccess.AppServer" value="{/Settings/ClientSettings/WinformClient/AppServerUrl[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <!-- PA Help URL -->
        <add key="PatientAccess.HelpURL" value="{/Settings/ClientSettings/WinformClient/HelpDocumentsUrl[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <!-- Default login credentials when running in Development -->
        <add key="ObtainDevCredentials" value="{/Settings/ClientSettings/WinformClient/UseCredentials[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <!-- Isolated Storage File Extension for the Crash Report -->
        <add key="CrashReportFileExtension" value=".pacr" />
        <!-- Client Isolated Storage File Name for a User  -->
        <add key="UserSettingsFileName" value="PatientAccessUserPreferences.xml" />
        <!-- Client Isolated Storage File Name for a Maching -->
        <add key="MachineSettingsFileName" value="PatientAccessSystemPreferences.xml" />
        <add key="ClientSettingsProvider.ServiceUri" value="" />
        <add key="OFFLINELOCK_REFRESH_INTERVAL" value="0:5:0.000" />
        <add key="PREOP_START_DATE" value="11/15/2009" />
        <add key="FLORIDA_SSN_RULE_START_DATE" value="03/23/2010" />
        <add key="CLINICALRESEARCHFIELDS_START_DATE" value="11/15/2009" />
        <add key="ALTERNATECAREFACILITY_START_DATE" value="01/03/2010" />
        <add key="RIGHTCARE_RIGHTPLACE_START_DATE" value="01/24/2010" />
        <add key="MODEOFARRIVALREQUIRED_START_DATE" value="02/24/2010" />
        <add key="PRIMARYCAREPHYSICIAN_START_DATE" value="03/01/2010" />
        <add key="PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE" value="08/23/2010" />
        <add key="ER_TO_IP_CONDITION_CODE_START_DATE" value="05/21/2010" />
        <add key="BIRTHTIME_START_DATE" value="11/01/2010" />
        <add key="MSE_BIRTHTIME_START_DATE" value="06/10/2012" />
        <add key="PATIENTPORTALOPT_START_DATE" value="07/10/2013"/>
        <add key="RIGHTTORESTRICT_START_DATE" value="12/01/2013" />
        <add key="EMAIL_ADDRESS_REQUIRED_START_DATE" value= "07/20/2014"/>
        <add key="HIECONSENT_START_DATE" value="{/Settings/ClientSettings/WinformClient/HIECONSENT_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="HOSPCOMMOPT_START_DATE" value ="{/Settings/ClientSettings/WinformClient/HOSPCOMMOPT_START_DATE[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}"/>
        <add key ="EMAILREASON_START_DATE" value="{/Settings/ClientSettings/WinformClient/EMAILREASON_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="GUARANTOR_DATEOFBIRTH_START_DATE" value="{/Settings/ClientSettings/WinformClient/GUARANTOR_DATEOFBIRTH_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="PRIMARYCAREPHYSICIAN_REQUIRED_START_DATE" value="{/Settings/ClientSettings/WinformClient/PRIMARYCAREPHYSICIAN_REQUIRED_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="SHARE_HIE_DATA_START_DATE" value="{/Settings/ClientSettings/WinformClient/SHARE_HIE_DATA_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="COB_IMFM_START_DATE" value="{/Settings/ClientSettings/WinformClient/COB_IMFM_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="PREREGISTRATION_IMFM_START_DATE" value="{/Settings/ClientSettings/WinformClient/PREREGISTRATION_IMFM_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="AUTO_COMPLETE_NO_LIABILITY_DUE_START_DATE" value="{/Settings/ClientSettings/WinformClient/AUTO_COMPLETE_NO_LIABILITY_DUE_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="PULL_PRIOR_VISIT_INSURANCE_PREMSE_START_DATE" value="{/Settings/ClientSettings/WinformClient/PULL_PRIOR_VISIT_INSURANCE_PREMSE_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key="AUTHORIZEPORTAL_START_DATE" value="{/Settings/ClientSettings/WinformClient/AUTHORIZEPORTAL_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key ="CELLPHONECONSENT_RULE_FOR_COSSIGNED_START_DATE" value="{/Settings/ClientSettings/WinformClient/CELLPHONECONSENT_RULE_FOR_COSSIGNED_START_DATE_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
      </appSettings>
      <!--
	    This is a temporary configuration for the 'first-time' remoting call to retrieve client configuration values from the server (web.config)
    	-->
      <system.runtime.remoting>
        <application>
          <channels>
            <channel ref="http">
              <clientProviders>
                <formatter ref="binary" />
                <provider type="PatientAccess.RemotingServices.VersionMatchingClientSinkProvider, PatientAccess.Common" ClientVersion="1.0" />
                <provider type="PatientAccess.RemotingServices.ErrorSinkProvider, PatientAccess.Common" />
              </clientProviders>
            </channel>
          </channels>
        </application>
      </system.runtime.remoting>
      <system.web>
        <membership defaultProvider="ClientAuthenticationMembershipProvider">
          <providers>
            <add name="ClientAuthenticationMembershipProvider" type="System.Web.ClientServices.Providers.ClientFormsAuthenticationMembershipProvider, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri="" />
          </providers>
        </membership>
        <roleManager defaultProvider="ClientRoleProvider" enabled="true">
          <providers>
            <add name="ClientRoleProvider" type="System.Web.ClientServices.Providers.ClientRoleProvider, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri="" cacheTimeout="86400" />
          </providers>
        </roleManager>
      </system.web>
    </configuration>
  </xsl:template>
</xsl:stylesheet>
