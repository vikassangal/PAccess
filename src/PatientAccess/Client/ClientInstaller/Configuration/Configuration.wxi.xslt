<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="Environment"  />
  <xsl:param name="Branch" />
  <xsl:param name="Version">1.0.0.0</xsl:param>
  <xsl:output indent="yes" />
  <xsl:template match="/" >
    <Include>
      <xsl:processing-instruction name="define">ProductName = "<xsl:value-of select="/Settings/ClientSettings/AppStart/ProductName[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()"/>"</xsl:processing-instruction>
      <xsl:processing-instruction name="define">ProductId = "<xsl:value-of select="/Settings/ClientSettings/AppStart/ProductID[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()"/>"</xsl:processing-instruction>
      <xsl:processing-instruction name="define">ProductVersion = "<xsl:value-of select="$Version"/>"</xsl:processing-instruction>
      <xsl:processing-instruction name="define">ProductDescription = "<xsl:value-of select="/Settings/ClientSettings/AppStart/ProductDescription[contains(@Environment,$Environment) and (@Branch=$Branch or @Branch='Any')]/text()"/>"</xsl:processing-instruction>
    </Include>
  </xsl:template>
</xsl:stylesheet>