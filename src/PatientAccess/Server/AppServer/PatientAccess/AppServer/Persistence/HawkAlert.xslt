<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	xmlns:msxml="urn:schemas-microsoft-com:xslt">
	<xsl:template match="/">	
		<table class="section" cellspacing="0" cellpadding="0">
			<tr class="subheading">
				<td style="border-color:Gray;border-right-style:solid;border-width:1px;">Alert</td>
				<td>Suggested Action</td>
			</tr>

			<xsl:for-each select="//HighRiskFraudAlert/Messages/Message">
			<tr>
				<td style="border-color:Gray;border-right-style:solid;border-bottom-style:solid;border-width:1px;" valign="top"><xsl:value-of select="Text"/><br/>
				<xsl:if test="string-length(Recommendation/Title) > 0">
					(<xsl:value-of select="Recommendation/Title"/>)
				</xsl:if>
				</td>
				<td style="border-color:Gray;border-bottom-style:solid;border-width:1px;" valign="top">
				<xsl:for-each select="Recommendation//Step">
					<xsl:value-of select="."/><br/><br/>
				</xsl:for-each>
				<xsl:if test="count(Recommendation//Step) = 0">
					No Suggested action available.
				</xsl:if>
				</td>
			</tr>
			</xsl:for-each>
		</table>
	</xsl:template>

</xsl:stylesheet>

  