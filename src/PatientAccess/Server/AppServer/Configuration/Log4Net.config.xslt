<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <log4net debug="false">
      <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
      <root>
        <level value="ERROR" />
        <appender-ref ref="ConfigAdoNetAppender" />
        <xsl:if test="$Environment='Local'">
          <appender-ref ref="OutputDebugStringAppender" />
        </xsl:if>
      </root>

      <!--Don't rename this logger without renaming the same logger on the client side-->
      <logger name="AuditLogger" additivity="true">
        <level value="INFO" />
      </logger>
      
      <logger name="PatientAccess.Jobs.SqlServerJob" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Jobs.DB2Job" additivity="true">
        <level value="INFO" />
      </logger>
      
      <logger name="PatientAccess.Persistence.FacilityBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.AccountPBARBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.AppServer" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.HDIEService" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.PatientPBARBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.DataValidationBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.GuarantorInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.InsuranceInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.PatientInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.CancelInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.DischargeInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.SwapBedInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.TransferInPatientToOutPatientInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.TransferToNewLocationInsertStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.SqlBuilderStrategy" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.TransactionCoordinator" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.TransactionBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.PbarUserBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.WorklistPBARBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.Utilities.IBMUtilities" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Rules.RuleEngine" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Services.EmsEventProcessing.PreRegistrationHandlerService" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.OnlinePreregistration.PreRegistrationSubmissionsBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Persistence.Locking.SqlOfflineLockBroker" additivity="true">
        <level value="INFO" />
      </logger>

      <logger name="PatientAccess.Jobs.PurgeOldOnlinePreregistrationSubmissions" additivity="true">
        <level value="INFO" />
      </logger>
      
      <appender name="ConfigAdoNetAppender" type="PatientAccess.Logging.ConfigAdoNetAppender, PatientAccess.Common">
        <bufferSize value="1" />
        <useTrasactions value="False" />
        <connectionType value="System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        <connectionStringName value="ConnectionString" />
        <commandType value="StoredProcedure" />
        <commandText value="Logging.SaveLogEntry" />
        <parameter>
          <parameterName value="@Host" />
          <dbType value="String" />
          <size value="255" />
          <layout type="log4net.Layout.PatternLayout">
		        <param name="ConversionPattern" type="log4net.Util.PatternString" value="%env{{COMPUTERNAME}}" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@Date" />
          <dbType value="DateTime" />
          <layout type="log4net.Layout.RawTimeStampLayout" />
        </parameter>
        <parameter>
          <parameterName value="@Thread" />
          <dbType value="String" />
          <size value="255" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%thread" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@Level" />
          <dbType value="String" />
          <size value="50" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%level" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@Logger" />
          <dbType value="String" />
          <size value="255" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%logger" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@Message" />
          <dbType value="String" />
          <size value="-1" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%message" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@SourceLocation" />
          <dbType value="String" />
          <size value="512" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%location" />
          </layout>
        </parameter>
        <parameter>
          <parameterName value="@Exception" />
          <dbType value="String" />
          <size value="-1" />
          <layout type="log4net.Layout.ExceptionLayout" />
        </parameter>
      </appender>

      <appender name="OutputDebugStringAppender" type="log4net.Appender.OutputDebugStringAppender" >
        <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
        </layout>
      </appender>
    
    </log4net>
  </xsl:template>
</xsl:stylesheet>
