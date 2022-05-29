<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exsl="http://exslt.org/common" extension-element-prefixes="exsl date" xmlns:date="http://exslt.org/dates-and-times">
  <xsl:param name="Environment" />
  <xsl:param name="Branch" />
  <xsl:output indent="yes" />
  <xsl:template match="/">
    <quartz xmlns="http://quartznet.sourceforge.net/JobSchedulingData" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="1.0" verwrite-existing-jobs="true">
      <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
      <job>
        <job-detail>
          <name>PurgeOldOnlinePreregistrationSubmissions</name>
          <group>Messaging</group>
          <description>Purge online preregistration submissions with admit date older than x days. The value of x is specified in the web.config file</description>
          <job-type>PatientAccess.Jobs.PurgeOldOnlinePreregistrationSubmissions, PatientAccess.AppServer</job-type>
          <volatile>false</volatile>
          <durable>false</durable>
          <recover>true</recover>
        </job-detail>
        <trigger>
          <cron>
            <name>PurgeOldOnlinePreregistrationSubmissionsTrigger</name>
            <group>Messaging</group>
            <description>Purge old online preregistration submissions every midnight</description>
            <misfire-instruction>DoNothing</misfire-instruction>
            <volatile>false</volatile>
            <job-name>PurgeOldOnlinePreregistrationSubmissions</job-name>
            <job-group>Messaging</job-group>
            <cron-expression>59 59 23 * * ?</cron-expression>
          </cron>
        </trigger>
      </job>      
      <job>
        <job-detail>
          <name>PurgeOrphanLocks</name>
          <group>Locking</group>
          <description>Purge application locks older than 20 minutes</description>
          <job-type>PatientAccess.Jobs.SqlServerJob, PatientAccess.AppServer</job-type>
          <volatile>false</volatile>
          <durable>false</durable>
          <recover>true</recover>
          <job-data-map>
            <entry>
              <key>ConnectionName</key>
              <value>ConnectionString</value>
            </entry>
            <entry>
              <key>CommandText</key>
              <value>EXECUTE Locking.DeleteLockEntriesOlderThan 20</value>
            </entry>
          </job-data-map>
        </job-detail>
        <trigger>
          <cron>
            <name>PurgeLocksTrigger</name>
            <group>Locking</group>
            <description>Purge locks every 5 minutes</description>
            <misfire-instruction>DoNothing</misfire-instruction>
            <volatile>false</volatile>
            <job-name>PurgeOrphanLocks</job-name>
            <job-group>Locking</job-group>
            <cron-expression>0 0/5 * * * ?</cron-expression>
          </cron>
        </trigger>
      </job>
      <job>
        <job-detail>
          <name>PurgeLogFiles</name>
          <group>Logging</group>
          <description>Purge application log entries older than 2 weeks</description>
          <job-type>PatientAccess.Jobs.SqlServerJob, PatientAccess.AppServer</job-type>
          <volatile>false</volatile>
          <durable>true</durable>
          <recover>false</recover>
          <job-data-map>
            <entry>
              <key>ConnectionName</key>
              <value>ConnectionString</value>
            </entry>
            <entry>
              <key>CommandText</key>
              <value>EXECUTE Logging.DeleteLogEntriesOlderThan 14</value>
            </entry>
          </job-data-map>
        </job-detail>
        <trigger>
          <cron>
            <name>PurgeLogsTrigger</name>
            <group>Logging</group>
            <description>Purge logs every midnight</description>
            <misfire-instruction>DoNothing</misfire-instruction>
            <volatile>false</volatile>
            <job-name>PurgeLogFiles</job-name>
            <job-group>Logging</job-group>
            <cron-expression>59 59 23 * * ?</cron-expression>
          </cron>
        </trigger>
      </job>
      <job>
        <job-detail>
          <name>PurgeCrashDumps</name>
          <group>CrashDump</group>
          <description>Purge crashdump entries older than 2 weeks</description>
          <job-type>PatientAccess.Jobs.SqlServerJob, PatientAccess.AppServer</job-type>
          <volatile>false</volatile>
          <durable>true</durable>
          <recover>false</recover>
          <job-data-map>
            <entry>
              <key>ConnectionName</key>
              <value>ConnectionString</value>
            </entry>
            <entry>
              <key>CommandText</key>
              <value>EXECUTE CrashDump.DeleteCrashReportsOlderThan 14</value>
            </entry>
          </job-data-map>
        </job-detail>
        <trigger>
          <cron>
            <name>PurgeCrashDumpTrigger</name>
            <group>CrashDump</group>
            <description>Purge dumps every midnight</description>
            <misfire-instruction>DoNothing</misfire-instruction>
            <volatile>false</volatile>
            <job-name>PurgeCrashDumps</job-name>
            <job-group>CrashDump</job-group>
            <cron-expression>59 59 23 * * ?</cron-expression>
          </cron>
        </trigger>
      </job>
      <xsl:if test="$Environment != 'Local'">
        <job>
          <job-detail>
            <name>PurgeOldQueuedEmployers</name>
            <group>EmployerManagement</group>
            <description>Purge employer entries older than 2 weeks</description>
            <job-type>PatientAccess.Jobs.PurgeNewEmployersJob, PatientAccess.AppServer</job-type>
            <volatile>false</volatile>
            <durable>true</durable>
            <recover>false</recover>
            <job-data-map>
              <entry>
                <key>ConnectionName</key>
                <value>DB2ConnectionTemplate</value>
              </entry>
              <entry>
                <key>MaxDays</key>
                <value>14</value>
              </entry>
            </job-data-map>
          </job-detail>
          <xsl:for-each select="/Settings/ExternalSystems/Hubs/Hub[contains(@Environment,$Environment)]">
          <trigger>
            <cron>
              <name>PurgeOldQueuedEmployersTrigger-<xsl:value-of select="@Name"/></name>
              <group>EmployerManagement</group>
              <description>Purge employers every midnight</description>
              <misfire-instruction>DoNothing</misfire-instruction>
              <volatile>false</volatile>
              <job-name>PurgeOldQueuedEmployers</job-name>
              <job-group>EmployerManagement</job-group>
              <job-data-map>
                <entry>
                  <key>Hub</key>
                  <xsl:element name="value">
                    <xsl:value-of select="@Name"/>
                  </xsl:element>
                </entry>
                <entry>
                  <key>Address</key>
                  <xsl:element name="value">
                    <xsl:value-of select="@Address"/>
                  </xsl:element>
                </entry>
              </job-data-map>
              <cron-expression>59 59 23 * * ?</cron-expression>
            </cron>
          </trigger>
          </xsl:for-each>
        </job>
        <job>
          <job-detail>
            <name>PurgeOldQueuedEmployerAddresses</name>
            <group>EmployerManagement</group>
            <description>Purge queued employer address entries older than 2 weeks</description>
            <job-type>PatientAccess.Jobs.PurgeNewEmployerAddressesJob, PatientAccess.AppServer</job-type>
            <volatile>false</volatile>
            <durable>true</durable>
            <recover>false</recover>
            <job-data-map>
              <entry>
                <key>ConnectionName</key>
                <value>DB2ConnectionTemplate</value>
              </entry>
              <entry>
                <key>MaxDays</key>
                <value>14</value>
              </entry>
            </job-data-map>
          </job-detail>
          <xsl:for-each select="/Settings/ExternalSystems/Hubs/Hub[contains(@Environment,$Environment)]">
            <trigger>
              <cron>
                <name>PurgeOldQueuedEmployerAddressTrigger-<xsl:value-of select="@Name"/></name>
                <group>EmployerManagement</group>
                <description>Purge employers every midnight</description>
                <misfire-instruction>DoNothing</misfire-instruction>
                <volatile>false</volatile>
                <job-name>PurgeOldQueuedEmployerAddresses</job-name>
                <job-group>EmployerManagement</job-group>
                <job-data-map>
                  <entry>
                    <key>Hub</key>
                    <xsl:element name="value">
                      <xsl:value-of select="@Name"/>
                    </xsl:element>
                  </entry>
                  <entry>
                    <key>Address</key>
                    <xsl:element name="value">
                      <xsl:value-of select="@Address"/>
                    </xsl:element>
                  </entry>
                </job-data-map>
                <cron-expression>59 59 23 * * ?</cron-expression>
              </cron>
            </trigger>
          </xsl:for-each>
        </job>        
      </xsl:if>
    </quartz>
  </xsl:template>
</xsl:stylesheet>
