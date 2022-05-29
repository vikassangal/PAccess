<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:date="http://exslt.org/dates-and-times" extension-element-prefixes="date">
  <xsl:param name="Environment" />
  <xsl:param name="Branch" />
  <xsl:param name="Version">1.0.0.0</xsl:param>
  <xsl:output indent="yes" />
  <xsl:template match="/">
  <downloads  xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <xsl:comment> Configuration generated for <xsl:value-of select="$Environment" /> on <xsl:value-of select="$Branch" /> (<xsl:value-of select="date:date-time()" />) </xsl:comment>
    <download>
      <name>Patient Access for Windows Version <xsl:value-of select="$Version"/></name>
      <installfile>PatientAccess.msi</installfile>
      <description>Installs the client software for the Patient Access system</description>
    </download>
  </downloads>
  </xsl:template>
</xsl:stylesheet>    