<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:variable name="LongCacheInterval" select="/Settings/ServerSettings/CacheTimeout/Long[contains(@Environment,$Environment)]/text()"/>
  <xsl:variable name="MediumCacheInterval" select="/Settings/ServerSettings/CacheTimeout/Medium[contains(@Environment,$Environment)]/text()" />
  <xsl:variable name="ShortCacheInterval" select="/Settings/ServerSettings/CacheTimeout/Short[contains(@Environment,$Environment)]/text()" />
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <configuration>
      <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
      <connectionStrings>
        <add name="ConnectionString" connectionString="{/Settings/Databases/SqlServer[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" providerName="System.Data.SqlClient" />
        <add name="DB2ConnectionTemplate" connectionString="{/Settings/Databases/Db2[contains(@Environment,$Environment)]/text()}" providerName="" />
      </connectionStrings>
      <appSettings>
        <add key="DownloadEnabled" value="{/Settings/ServerSettings/AppUpdater/DownloadEnabled[contains(@Environment,$Environment)]/text()}" />
        <add key="ClientDownloadURL" value="{/Settings/ServerSettings/AppUpdater/ClientDownloadUrl[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key="ClientDownloadPath" value="{/Settings/ServerSettings/AppUpdater/ClientDownloadPath[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />        
        <xsl:comment> These EDV settings, when not blank, override the user and facility passed to Passport for data validation. </xsl:comment>
        <add key="EdvUserOverride" value="{/Settings/ExternalSystems/Edv/Override/User[contains(@Environment,$Environment)]/text()}" />
        <add key="EdvFacilityOverride" value="{/Settings/ExternalSystems/Edv/Override/Facility[contains(@Environment,$Environment)]/text()}" />
        <add key="LockWaitAttempts" value="10" />
        <add key="LockWaitInterval" value="500" />
        <add key="MSP2_START_DATE" value="11/01/2006" />
        <add key="UseDynamicSearch" value="true" />
        <add key="RetryAttempts" value="5" />
        <add key="LOADINGFACILITY" value="" />
        <add key="ConnectionTimeout" value="0" />
        <add key="CommandTimeout" value="0" />
        <add key="EMPLOYER_CODE_DATA_AREA_NAME" value="EM#       " />
        <add key="EMPLOYER_CODE_LIB_NAME" value="PALIBR    " />
        <add key="VersionMatchEnforced" value="true" />
        <add key="ACTIVITY_TIMEOUT_FIRST_ALERT" value="3300000" />
        <add key="ACTIVITY_TIMEOUT_SECOND_ALERT" value="300000" />
        <add key="ADAMApplicationName" value="PatientAccess" />
        <add key="ADAMLegacyApplicationName" value="PBAR" />
        <add key="OrganizationalService.RenewFrequency" value="1440" />
        <add key="OrganizationalService.DeactivateFacilities" value="false" />
        <add key="OrganizationalService.Provider.Assembly" value="PatientAccess.AppServer.dll" />
        <add key="OrganizationalService.Provider.Class" value="PatientAccess.Persistence.OrganizationalHierarchyProvider" />
        <add key="PatientAccess.IsServer" value="True" />
        <add key="InsertImmediate" value="true" />
        <add key="UseMedicareCannotBePrimaryPayorForAutoAccidentRule" value="true" />
        <add key="PREOP_START_DATE" value="11/01/2009" />
        <add key="CACHE_KEY_FOR_ACCOMODATIONCODES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ACTIVITY_CODES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ACTIVITIESLOADED_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ADMITSOURCES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_ALLACCOMODATIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ALLACTIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ALLRULES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ALLRULESBYID_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_BENEFITS_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_CONDITIONCODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_CONDITIONOFSERVICE_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_CONFIDENTIALCODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_CONNECTION_SPECS_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_COUNTIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_COUNTRIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_DISCHARGE_DISPOSITIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_DISCHARGE_STATUSES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_EMPLOYMENT_STATUSES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ETHNICITIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_FACILITIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_FINANCIALCLASSES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_CREDITCARDTYPES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_FOLLOWUPUNITS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_GENDERS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_HOSPITALCLINICS_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_HSVS_INTERVAL" value="{$ShortCacheInterval}" />
        <add key="CACHE_KEY_FOR_LANGUAGES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_MARITALSTATUSES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_MODESOFARRIVAL_INTERVAL" value="{$ShortCacheInterval}" />
        <add key="CACHE_KEY_FOR_NPPVERSIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_NURSINGSTATIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_OCCURANCECODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_PATIENTTYPES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_PHYSICIANROLES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_PHYSICIANSPECIALTIES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_PRETESTHOSPITALCLINICS_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_RACES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_READMITCODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_REFERRALFACILITIES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_REFERRALSOURCES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_REFERRALTYPES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_RELIGIONS_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_ROLES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ROOMS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_RULEACTIONS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_RULEWORKLIST_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_SCHEDULECODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_SELECTABLEOCCURANCECODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_SPANCODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_STATES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_TENET_PLAN_CODES_INTERVAL" value="{$MediumCacheInterval}" />
        <add key="CACHE_KEY_FOR_TYPESOFRELATIONSHIPS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_WORKLISTRANGES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_WORKLISTS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_WORKLISTSELECTIONRANGES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_WRITEABLE_ACTIVITY_CODES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ZIPCODESTATUSES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_TYPESOFVERIFICATIONRULES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_TYPESOFPRODUCTS_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_RESEARCHSTUDIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_ALTERNATECAREFACILITIES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_SUFFIXCODES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="CACHE_KEY_FOR_DIALYSISCENTERNAMES_INTERVAL" value="{$LongCacheInterval}" />
        <add key="PARSE_COMMERCIAL" value="true" />
        <add key="PARSE_GOVERNMENT_MEDICARE" value="true" />
        <add key="PARSE_GOVERNMENT_MEDICAID" value="false" />
        <add key="PARSE_GOVERNMENT_OTHER" value="false" />
        <add key="RegexPatternForMedicaidPayorId" value="(?:MEDICAID-CA)" />
        <add key="RegexPatternForCommercialPayorId" value="(?:CIGNA.*|06-0303370*)" />
        <add key="LOADINGFOLLOWUPUNIT" value="000" />
        <add key="LOADCACHESERIAL" value="true" />
        <add key="PERFORM_PRESAVECHECK" value="true" />
        <add key="WEB_REQUEST_DEFAULTS" value="false" />
        <add key="WEB_REQUEST_KEEP_ALIVE" value="false" />
        <add key="WEB_REQUEST_PROTOCOL_VERSION" value="1.1" />
        <add key="CCMaxServicePointIdleTime" value="3000" />
        <add key="CCCheckForCrossThreads" value="true" />
        <add key="CCAnnouncementsAfterCrash" value="true" />
        <add key="CCUseRemotingCompression" value="true" />
        <add key="CCRemotingErrorRetries" value="0" />
        <add key="CCRemotingTimeout" value="-1" />
        <add key="CCApplicationInActivityTimeout" value="900000" />
        <add key="CIEGUID" value="{/Settings/ExternalSystems/Hdi/Guid[contains(@Environment,$Environment)]/text()}" />
        <add key="PatientAccessServiceEngine" value="{/Settings/ExternalSystems/Hdi/Url[contains(@Environment,$Environment)]/text()}" />
        <add key="DB2UTIL_USER_NAME" value="PACCESS" />
        <add key="DB2UTIL_PASSWORD" value="{/Settings/Databases/PAS_PASSWORD_SET[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" />
        <add key="ADAMAccessKey" value="{/Settings/ExternalSystems/Adam/Key[contains(@Environment,$Environment)]/text()}" />
        <add key="ADAMApplicationGUID" value="{/Settings/ExternalSystems/Adam/Guid[contains(@Environment,$Environment)]/text()}" />
        <add key="ADAM.SecurityService.URL" value="{/Settings/ExternalSystems/Adam/Url[contains(@Environment,$Environment)]/text()}" />
        <add key="ADAM.SecurityService.Timeout" value="{/Settings/ExternalSystems/Adam/Timeout[contains(@Environment,$Environment)]/text()}" />
        <add key="UserNameDefault" value="{/Settings/ServerSettings/LoginDefaults/User[contains(@Environment,$Environment)]/text()}" />
        <add key="PasswordDefault" value="{/Settings/ServerSettings/LoginDefaults/Password[contains(@Environment,$Environment)]/text()}" />
        <add key="WorkstationIDDefault" value="{/Settings/ServerSettings/LoginDefaults/Workstation[contains(@Environment,$Environment)]/text()}" />
        <add key="CFDBLookupServiceUrl" value="{/Settings/ExternalSystems/Cfdb/Url[contains(@Environment,$Environment)]/text()}" />
        <add key="AddressValidationServiceUrl" value="{/Settings/ExternalSystems/Edv/AddressValidationServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="BenefitsValidationServiceUrl" value="{/Settings/ExternalSystems/Edv/BenefitsValidation5010ServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="BenefitsValidationFusServiceUrl" value="{/Settings/ExternalSystems/Edv/BenefitsValidationFusServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="ComplianceCheckerServiceUrl" value="{/Settings/ExternalSystems/Edv/ComplianceCheckerServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="CreditValidationServiceUrl" value="{/Settings/ExternalSystems/Edv/CreditValidationServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="PriorAccountBalanceServiceUrl" value="{/Settings/ExternalSystems/Edv/PriorAccountBalanceServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="PatientAccess.UI.DocumentImagingViews.ScanURL" value="{/Settings/ExternalSystems/DocumentImaging/ScanUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="PatientAccess.UI.DocumentImagingViews.ViewURL" value="{/Settings/ExternalSystems/DocumentImaging/ViewUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="viServiceUrl" value="{/Settings/ExternalSystems/DocumentImaging/DocumentServiceUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="VIWebPreviousVisitServiceUrl" value="{/Settings/ExternalSystems/DocumentImaging/PreviousVisitUrl[contains(@Environment,$Environment)]/text()}" />
        <add key="VIWebGUID" value="{/Settings/ExternalSystems/DocumentImaging/Key[contains(@Environment,$Environment)]/text()}" />
        <add key="VIWebAppKey" value="{/Settings/ExternalSystems/DocumentImaging/Guid[contains(@Environment,$Environment)]/text()}" />
        <add key="USING_PATIENTNAME_MANGLEDNAMES" value="0" />
        <add key="USING_GUARANTORNAME_MANGLEDNAMES" value="0" />
        <add key="USING_PHYSICIANNAME_MANGLEDNAMES" value="0" />
        <add key="USING_IPANAME_MANGLEDNAMES" value="0" />
        <add key="USING_BROKERNAME_MANGLEDNAMES" value="0" />
        <add key="USING_COVEREDGROUPNAME_MANGLEDNAMES" value="0" />
        <add key="USING_PAYORNAME_MANGLEDNAMES" value="0" />        
      </appSettings>
      <system.net>
        <defaultProxy>
          <bypasslist>
            <add address="[a-z]+\.hdc\.net$" />
            <add address="[a-z]+\.mdltenethealth\.net$" />
            <add address="[a-z]+\.tenethealth\.net$" />
            <add address="[a-z]+\.tenethealth\.com$" />
            <add address="[a-z]+\.etenet\.com$" />
          </bypasslist>
        </defaultProxy>
      </system.net>
      <system.serviceModel>
        <bindings>
          <basicHttpBinding>
            <binding name="BenefitsValidation5010ServiceSEIBinding" closeTimeout="00:01:00"
              openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00"
              allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard"
              maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536"
              messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered"
              useDefaultWebProxy="true">
              <readerQuotas maxDepth="32" maxStringContentLength="2147483647" maxArrayLength="16384"
                maxBytesPerRead="4096" maxNameTableCharCount="16384" />
              <security mode="None">
                <transport clientCredentialType="None" proxyCredentialType="None"
                  realm="" />
                <message clientCredentialType="UserName" algorithmSuite="Default" />
              </security>
            </binding>
          </basicHttpBinding>
        </bindings>
        <client>
          <endpoint address="{/Settings/ExternalSystems/Edv/BenefitsValidation5010ServiceUrl[contains(@Environment,$Environment)]/text()}"
            binding="basicHttpBinding" bindingConfiguration="BenefitsValidation5010ServiceSEIBinding"
            contract="BenefitsValidation5010Proxy.BenefitsValidation5010ServiceSEI"
            name="BenefitsValidation5010ServiceSEIPort" />
        </client>
      </system.serviceModel>
    </configuration>
  </xsl:template>
</xsl:stylesheet>