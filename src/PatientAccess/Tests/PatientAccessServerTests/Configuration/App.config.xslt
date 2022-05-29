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
      <configSections>
        <section name="encryptionKeys" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0,Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        <section name="quartz" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0,Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        <sectionGroup name="common">
          <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
        </sectionGroup>
      </configSections>
      <connectionStrings>
        <add name="ConnectionString" connectionString="{/Settings/Databases/SqlServer[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()}" providerName="System.Data.SqlClient" />
        <add name="DB2ConnectionTemplate" connectionString="{/Settings/Databases/Db2[contains(@Environment,$Environment)]/text()}" providerName="" />
      </connectionStrings>

      <encryptionKeys>
        <add key="OnlinePreRegSubmissionsKeyAsBase64String" value="YdiRtgjMZEdeqDpK+jsCvQ==" />
      </encryptionKeys>
      
      <xsl:comment> Quartz.NET is a job scheduler built into the AppServer </xsl:comment>
      <quartz>
        <add key="quartz.scheduler.instanceName" value="PatientAccess" />
        <add key="quartz.scheduler.instanceId" value="AUTO" />
        <add key="quartz.threadPool.type" value="Quartz.Simpl.SimpleThreadPool, Quartz" />
        <add key="quartz.threadPool.threadCount" value="5" />
        <add key="quartz.threadPool.threadPriority" value="2" />
        <add key="quartz.jobStore.misfireThreshold" value="60000" />
        <add key="quartz.jobStore.type" value="Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" />
        <add key="quartz.jobStore.driverDelegateType" value="Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" />
        <add key="quartz.jobStore.useProperties" value="true" />
        <add key="quartz.jobStore.acquireTriggersWithinLock" value="true" />
        <add key="quartz.jobStore.tablePrefix" value="Quartz." />
        <add key="quartz.jobStore.dataSource" value="primaryDataSource" />
        <add key="quartz.jobStore.clustered" value="true" />
        <add key="quartz.jobStore.lockHandler.type" value="Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz" />
        <add key="quartz.dataSource.primaryDataSource.provider" value="SqlServer-20" />
        <add key="quartz.dataSource.primaryDataSource.connectionStringName" value="ConnectionString" />
        <add key="quartz.plugin.xml.type" value="Quartz.Plugin.Xml.JobInitializationPlugin, Quartz" />
        <add key="quartz.plugin.xml.fileNames" value="~/QuartzJobs.xml" />
      </quartz>
      <xsl:comment> Quartz.NET uses Commons.Logging for output. This section bridges that to our existing Log4net instance </xsl:comment>
      <common>
        <logging>
          <factoryAdapter type="Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter, Common.Logging.Log4net">
            <arg key="configType" value="EXTERNAL" />
          </factoryAdapter>
        </logging>
      </common>
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
        <add key="ACTIVITY_TIMEOUT_FIRST_ALERT" value="1620000" />
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
        <add key="FLORIDA_SSN_RULE_START_DATE" value="03/23/2010" />
        <add key="CLINICALRESEARCHFIELDS_START_DATE" value="11/15/2009" />
        <add key="ALTERNATECAREFACILITY_START_DATE" value="01/03/2010" />
        <add key="RIGHTCARE_RIGHTPLACE_START_DATE" value="01/24/2010" />
        <add key="MODEOFARRIVALREQUIRED_START_DATE" value="02/24/2010" />
        <add key="PRIMARYCAREPHYSICIAN_START_DATE" value="03/01/2010" />
        <add key="THRESHOLD_FOR_PULLING_PCP_FORWARD_IN_DAYS" value="60" />
        <add key="PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE" value="08/23/2010" />
        <add key="ER_TO_IP_CONDITION_CODE_START_DATE" value="05/21/2010" />
        <add key="BIRTHTIME_START_DATE" value="11/01/2010" />
        <add key="MSE_BIRTHTIME_START_DATE" value="06/10/2012" />
        
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
        <add key="IdentityHubServiceUrl" value="{/Settings/ExternalSystems/Empi/IdentityHubServiceUrl[contains(@Environment,$Environment)]/text()}" />
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
        <add key="OKTAAUTHURL" value="{/Settings/ExternalSystems/OKTA/AuthUrl[contains(@Environment,$Environment)]/text()}"/>
        <add key="OKTAROLEURL" value="{/Settings/ExternalSystems/OKTA/RoleUrl[contains(@Environment,$Environment)]/text()}"/>
        <add key="OKTAAPIKey" value="{/Settings/ExternalSystems/OKTA/APIKey[contains(@Environment,$Environment)]/text()}"/>
        <add key="OKTAUSERINFOURL" value="{/Settings/ExternalSystems/OKTA/UserInfo[contains(@Environment,$Environment)]/text()}"/>        
        <add key="CONFIG_PROXY_URL" value="{/Settings/ExternalSystems/OKTA/CONFIG_PROXY_URL[contains(@Environment,$Environment)]/text()}"/>      
        <add key="PROXY_USER" value="{/Settings/ExternalSystems/OKTA/PROXY_USER[contains(@Environment,$Environment)]/text()}"/>      
        <add key="PROXY_PASSWORD" value="{/Settings/ExternalSystems/OKTA/PROXY_PASSWORD[contains(@Environment,$Environment)]/text()}"/>                
        <add key="LEGACY_URL" value="{/Settings/ExternalSystems/OKTA/Legacy_URL[contains(@Environment,$Environment)]/text()}"/>  
        <add key="LEGACY_USER" value="{/Settings/ExternalSystems/OKTA/Legacy_USER[contains(@Environment,$Environment)]/text()}"/>  
        <add key="LEGACY_PASSWORD" value="{/Settings/ExternalSystems/OKTA/Legacy_PASSWORD[contains(@Environment,$Environment)]/text()}"/>
        <add key="PASServerEnvironment" value="{/Settings/ServerSettings/PASServerEnvironmentSettings/PASServerEnvironment[contains(@Environment,$Environment)]/text()}" />
        <add key="IsFallOutToADAM" value="{/Settings/ExternalSystems/OKTA/IsFallOutToADAM[contains(@Environment,$Environment)]/text()}"/>
      </appSettings>
      <system.runtime.remoting>
        <application>
          <service>
            <!-- AddressPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AddressPBARBroker, PatientAccess.AppServer" objectUri="AddressPBARBroker.rem" />
            <!-- AccountPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.Specialized.ClinicalTrialsAccountPBARBroker, PatientAccess.AppServer" objectUri="AccountPBARBroker.rem" />
            <!-- ClienConfigurationBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ClientConfigurationBroker, PatientAccess.AppServer" objectUri="ClientConfigurationBroker.rem" />
            <!-- CopyAccountCommandFactory -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AccountCopyBroker, PatientAccess.AppServer" objectUri="AccountCopyBroker.rem" />
            <!-- EmployerPbarBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.EmployerPbarBroker, PatientAccess.AppServer" objectUri="EmployerPbarBroker.rem" />
            <!-- EmploymentStatusBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.EmploymentStatusPBARBroker, PatientAccess.AppServer" objectUri="EmploymentStatusPBARBroker.rem" />
            <!-- FacilityBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.FacilityBroker, PatientAccess.AppServer" objectUri="FacilityBroker.rem" />
            <!-- FollowUpUnitBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.FollowUpUnitBroker, PatientAccess.AppServer" objectUri="FollowUpUnitBroker.rem" />
            <!-- LoggerBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.LoggerBroker, PatientAccess.AppServer" objectUri="LoggerBroker.rem" />
            <!-- FinancialBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.FinancialClassesPBARBroker, PatientAccess.AppServer" objectUri="FinancialClassesPBARBroker.rem" />
            <!-- FusNoteBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.FusNoteBroker, PatientAccess.AppServer" objectUri="FusNoteBroker.rem" />
            <!-- DemographicsPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.DemographicsPBARBroker, PatientAccess.AppServer" objectUri="DemographicsPBARBroker.rem" />
            <!-- OriginBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.OriginPBARBroker, PatientAccess.AppServer" objectUri="OriginPBARBroker.rem" />
            <!-- ReligionPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ReligionPBARBroker, PatientAccess.AppServer" objectUri="ReligionPBARBroker.rem" />
            <!-- SSNPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.SSNPBARBroker, PatientAccess.AppServer" objectUri="SSNPBARBroker.rem" />
            <!-- GuarantorBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.GuarantorPBARBroker, PatientAccess.AppServer" objectUri="GuarantorPBARBroker.rem" />
            <!-- PatientPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.Specialized.ClinicalTrialsPatientPBARBroker, PatientAccess.AppServer" objectUri="PatientPBARBroker.rem" />
            <!-- RelationshipTypeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.RelationshipTypePBARBroker, PatientAccess.AppServer" objectUri="RelationshipTypePBARBroker.rem" />
            <!-- InsuranceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.InsurancePBARBroker, PatientAccess.AppServer" objectUri="InsurancePBARBroker.rem" />
            <!-- OccuranceCodePBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.OccuranceCodePBARBroker, PatientAccess.AppServer" objectUri="OccuranceCodePBARBroker.rem" />
            <!-- IWorklistBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.WorklistPBARBroker, PatientAccess.AppServer" objectUri="WorklistPBARBroker.rem" />
            <!-- IWorklistPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.WorklistSettingsBroker, PatientAccess.AppServer" objectUri="WorklistSettingsBroker.rem" />
            <!-- LocationPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.LocationPBARBroker, PatientAccess.AppServer" objectUri="LocationPBARBroker.rem" />
            <!-- DischargeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.DischargePBARBroker, PatientAccess.AppServer" objectUri="DischargePBARBroker.rem" />
            <!-- PhysicianPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.PhysicianPBARBroker, PatientAccess.AppServer" objectUri="PhysicianPBARBroker.rem" />
            <!-- HDIService -->
            <wellknown mode="SingleCall" type="PatientAccess.Services.HDIService, PatientAccess.AppServer" objectUri="HDIService.rem" />
            <!-- RuleBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.RuleBroker, PatientAccess.AppServer" objectUri="RuleBroker.rem" />
            <!-- CreditCardTypeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.CreditCardTypeBroker, PatientAccess.AppServer" objectUri="CreditCardTypeBroker.rem" />
            <!-- HSVBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.HSVPBARBroker, PatientAccess.AppServer" objectUri="HSVPBARBroker.rem" />
            <!-- AccountNumberPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AccountNumberPBARBroker, PatientAccess.AppServer" objectUri="AccountNumberPBARBroker.rem" />
            <!-- TransactionBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TransactionBroker, PatientAccess.AppServer" objectUri="TransactionBroker.rem" />
            <!-- HospitalClinicsBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.HospitalClinicsPBARBroker, PatientAccess.AppServer" objectUri="HospitalClinicsPBARBroker.rem" />
            <!-- ModeOfArrivalPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ModeOfArrivalPBARBroker, PatientAccess.AppServer" objectUri="ModeOfArrivalPBARBroker.rem" />
            <!-- AdmitSourceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AdmitSourcePBARBroker, PatientAccess.AppServer" objectUri="AdmitSourcePBARBroker.rem" />
            <!-- TimeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TimeBroker, PatientAccess.AppServer" objectUri="TimeBroker.rem" />
            <!-- FacllityFlagBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.FacilityFlagPBARBroker, PatientAccess.AppServer" objectUri="FacilityFlagPBARBroker.rem" />
            <!-- ConditionOfServiceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ConditionOfServiceBroker, PatientAccess.AppServer" objectUri="ConditionOfServiceBroker.rem" />
            <!-- ConfidentialCodeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ConfidentialCodePBARBroker, PatientAccess.AppServer" objectUri="ConfidentialCodePBARBroker.rem" />
            <!-- NPPVersionBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.NPPVersionPBARBroker, PatientAccess.AppServer" objectUri="NPPVersionPBARBroker.rem" />
            <!-- InfoReceivedSourceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.InfoReceivedSourceBroker, PatientAccess.AppServer" objectUri="InfoReceivedSourceBroker.rem" />
            <!-- TypeOfProductBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TypeOfProductBroker, PatientAccess.AppServer" objectUri="TypeOfProductBroker.rem" />
            <!-- BenefitsCategoryPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.BenefitsCategoryPBARBroker, PatientAccess.AppServer" objectUri="BenefitsCategoryPBARBroker.rem" />
            <!-- TypeOfVerificationRuleBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TypeOfVerificationRuleBroker, PatientAccess.AppServer" objectUri="TypeOfVerificationRuleBroker.rem" />
            <!-- RoleBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.RoleBroker, PatientAccess.AppServer" objectUri="RoleBroker.rem" />
            <!-- AnnouncementBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AnnouncementBroker, PatientAccess.AppServer" objectUri="AnnouncementBroker.rem" />
            <!-- VIWebServiceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.VIWebServiceBroker, PatientAccess.AppServer" objectUri="VIWebServiceBroker.rem" />
            <!-- SpanCodePBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.SpanCodePBARBroker, PatientAccess.AppServer" objectUri="SpanCodePBARBroker.rem" />
            <!-- UserBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.UserBroker, PatientAccess.AppServer" objectUri="UserBroker.rem" />
            <!-- ConditionCodeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ConditionCodePBARBroker, PatientAccess.AppServer" objectUri="ConditionCodePBARBroker.rem" />
            <!-- EmailReasonBroker-->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.EmailReasonBroker, PatientAccess.AppServer" objectUri="EmailReasonBroker.rem" />
            <!-- DataValidationBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.DataValidationBroker, PatientAccess.AppServer" objectUri="DataValidationBroker.rem" />
            <!-- ReferralSourceBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ReferralSourcePBARBroker, PatientAccess.AppServer" objectUri="ReferralSourcePBARBroker.rem" />
            <!-- ScheduleCodePBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ScheduleCodePBARBroker, PatientAccess.AppServer" objectUri="ScheduleCodePBARBroker.rem" />
            <!-- ReAdmitCodeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ReAdmitCodePBARBroker, PatientAccess.AppServer" objectUri="ReAdmitCodePBARBroker.rem" />
            <!-- ReferralFacilityPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ReferralFacilityPBARBroker, PatientAccess.AppServer" objectUri="ReferralFacilityPBARBroker.rem" />
            <!-- ReferralTypePBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ReferralTypePBARBroker, PatientAccess.AppServer" objectUri="ReferralTypePBARBroker.rem" />
            <!-- PBARSecurityCodeBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.PBARSecurityCodeBroker, PatientAccess.AppServer" objectUri="PBARSecurityCodeBroker.rem" />
            <!-- MSPPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.MSPPBARBroker, PatientAccess.AppServer" objectUri="MSPPBARBroker.rem" />
            <!-- CrashReportBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.CrashReportBroker, PatientAccess.AppServer" objectUri="CrashReportBroker.rem" />
            <!-- AuthorizationStatusPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.AuthorizationStatusPBARBroker, PatientAccess.AppServer" objectUri="AuthorizationStatusPBARBroker.rem" />
            <!-- MedicalRecordNumberBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.MedicalRecordNumberPBARBroker, PatientAccess.AppServer" objectUri="MedicalRecordNumberPBARBroker.rem" />
            <!-- TransactionCoordinator -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TransactionCoordinator, PatientAccess.AppServer" objectUri="TransactionCoordinator.rem" />
            <!-- CodedDiagnosisPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.CodedDiagnosisPBARBroker, PatientAccess.AppServer" objectUri="CodedDiagnosisPBARBroker.rem" />
            <!-- TimeoutBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.TimeoutBroker, PatientAccess.AppServer" objectUri="TimeoutBroker.rem" />
            <!-- SqlOfflineLockBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.Locking.SqlOfflineLockBroker, PatientAccess.AppServer" objectUri="SqlOfflineLockBroker.rem" />
            <!-- ResearchStudyPBARBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.ResearchStudyPBARBroker, PatientAccess.AppServer" objectUri="ResearchStudyPBARBroker.rem" />
            <!-- LeftOrStayedBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.LeftOrStayedBroker, PatientAccess.AppServer" objectUri="LeftOrStayedBroker.rem" />
            <!-- PreRegistrationMessageBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.OnlinePreregistration.PreRegistrationSubmissionsBroker, PatientAccess.AppServer" objectUri="PreRegistrationSubmissionsBroker.rem" />
            <!-- OKTAUserBroker -->
            <wellknown mode="SingleCall" type="PatientAccess.Persistence.OKTAUserBroker, PatientAccess.AppServer" objectUri="OKTAUserBroker.rem" />
			<!-- DOFRInitiateBroker -->
			<wellknown mode="SingleCall" type="PatientAccess.Persistence.DOFRInitiateBroker, PatientAccess.AppServer" objectUri="DOFRInitiateBroker.rem" />
			<!-- ServiceCategoryBroker -->
			<wellknown mode="SingleCall" type="PatientAccess.Persistence.ServiceCategoryBroker, PatientAccess.AppServer" objectUri="ServiceCategoryBroker.rem" />
			<!-- AidCodeBroker -->
			<wellknown mode="SingleCall" type="PatientAccess.Persistence.AidCodeBroker, PatientAccess.AppServer" objectUri="AidCodeBroker.rem" />

		  </service>
          <channels>
            <channel ref="http">
              <clientProviders>
                <formatter ref="binary" />
              </clientProviders>
              <serverProviders>
                <provider type="PatientAccess.RemotingServices.VersionMatchingServerSinkProvider, PatientAccess.Common" ClientVersion="1.0" />
                <provider type="PatientAccess.RemotingServices.GZipCompressionServerSinkProvider, PatientAccess.Common" Enable="true" />
                <formatter ref="binary" typeFilterLevel="Full" />
              </serverProviders>
            </channel>
          </channels>
        </application>
      </system.runtime.remoting>
      <system.web>
        <!--  DYNAMIC DEBUG COMPILATION
            Set compilation debug="true" to enable ASPX debugging.  Otherwise, setting this value to
            false will improve runtime performance of this application. 
            Set compilation debug="true" to insert debugging symbols (.pdb information)
            into the compiled page. Because this creates a larger file that executes
            more slowly, you should set this value to true only when debugging and to
            false at all other times. For more information, refer to the documentation about
            debugging ASP.NET files.
        -->
        <compilation defaultLanguage="c#" debug="true" />
        <!--  CUSTOM ERROR MESSAGES
            Set customErrors mode="On" or "RemoteOnly" to enable custom error messages, "Off" to disable. 
            Add <error> tags for each of the errors you want to handle.

            "On" Always display custom (friendly) messages.
            "Off" Always display detailed ASP.NET error information.
            "RemoteOnly" Display custom (friendly) messages only to users not running 
            on the local Web server. This setting is recommended for security purposes, so 
            that you do not display application detail information to remote clients.
        -->
        <customErrors mode="Off" />
        <!--  AUTHENTICATION 
            This section sets the authentication policies of the application. Possible modes are "Windows", 
            "Forms", "Passport" and "None"

            "None" No authentication is performed. 
            "Windows" IIS performs authentication (Basic, Digest, or Integrated Windows) according to 
            its settings for the application. Anonymous access must be disabled in IIS. 
            "Forms" You provide a custom form (Web page) for users to enter their credentials, and then 
            you authenticate them in your application. A user credential token is stored in a cookie.
            "Passport" Authentication is performed via a centralized authentication service provided
            by Microsoft that offers a single logon and core profile services for member sites.
        -->
        <authentication mode="Windows" />
        <!--  AUTHORIZATION 
            This section sets the authorization policies of the application. You can allow or deny access
            to application resources by user or role. Wildcards: "*" mean everyone, "?" means anonymous 
            (unauthenticated) users.
        -->
        <authorization>
          <allow users="*" />
          <!-- Allow all users -->
          <!--  <allow     users="[comma separated list of users]"
                             roles="[comma separated list of roles]"/>
                  <deny      users="[comma separated list of users]"
                             roles="[comma separated list of roles]"/>
            -->
        </authorization>
        <!--  APPLICATION-LEVEL TRACE LOGGING
            Application-level tracing enables trace log output for every page within an application. 
            Set trace enabled="true" to enable application trace logging.  If pageOutput="true", the
            trace information will be displayed at the bottom of each page.  Otherwise, you can view the 
            application trace log by browsing the "trace.axd" page from your web application
            root. 
        -->
        <trace enabled="false" requestLimit="10" pageOutput="false" traceMode="SortByTime" localOnly="false" />
        <!--  SESSION STATE SETTINGS
            By default ASP.NET uses cookies to identify which requests belong to a particular session. 
            If cookies are not available, a session can be tracked by adding a session identifier to the URL. 
            To disable cookies, set sessionState cookieless="true".
        -->
        <sessionState mode="InProc" stateConnectionString="tcpip=127.0.0.1:42424" sqlConnectionString="data source=127.0.0.1;Trusted_Connection=yes" cookieless="false" timeout="20" />
        <!--  GLOBALIZATION
            This section sets the globalization settings of the application. 
        -->
        <globalization requestEncoding="utf-8" responseEncoding="utf-8" />
        <!-- MAX REQUEST LENGTH
            This value limits the maximum request size that will be accepted by the HTTP runtime.  Setting this value too small
            will affect the transfer of Crash Reports from client workstations to the CrashReportingService.
            Currently set to 8 MB Maximum.
        -->
        <httpRuntime maxRequestLength="8192" />
        <httpModules>
          <add name="UnhandledExceptionModule" type="PatientAccess.Http.UnhandledExceptionModule, PatientAccess.AppServer" />
        </httpModules>
      </system.web>
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
              maxBufferSize="2147483647" maxBufferPoolSize="2147483647" maxReceivedMessageSize="2147483647"
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
            <binding name="IdentityHubSoapBinding"  maxReceivedMessageSize="2147483647"/>
            <binding name="PriorAccountBalanceServiceSoap" closeTimeout="00:01:00"
                     openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00"
                     allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard"
                     maxBufferSize="2147483647" maxBufferPoolSize="2147483647" maxReceivedMessageSize="2147483647"
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
            contract="BenefitsValidation5010Proxy.BenefitsValidation5010ServiceSoap"
            name="BenefitsValidation5010ServiceSEIPort" />
          <endpoint address="{/Settings/ExternalSystems/Empi/IdentityHubServiceUrl[contains(@Environment,$Environment)]/text()}"
                binding="basicHttpBinding" bindingConfiguration="IdentityHubSoapBinding"
                contract="IdentityHubService.IdentityHubPort" name="IdentityHub" />
          <endpoint address="{/Settings/ExternalSystems/Edv/PriorAccountBalanceServiceUrl[contains(@Environment,$Environment)]/text()}"
                    binding="basicHttpBinding" bindingConfiguration="PriorAccountBalanceServiceSoap"
                    contract="PriorAccountBalanceProxy.PriorAccountBalanceServiceSoap"
                    name="PriorAccountBalanceServiceSoap" />
        </client>
      </system.serviceModel>
    </configuration>
  </xsl:template>
</xsl:stylesheet>
