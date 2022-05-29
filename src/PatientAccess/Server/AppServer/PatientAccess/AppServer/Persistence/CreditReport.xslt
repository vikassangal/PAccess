<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	xmlns:msxml="urn:schemas-microsoft-com:xslt" xmlns:utils="http://patientaccess.com">
	<xsl:template match="//Subject">	
		<table width="100%">
			<tr><td><xsl:apply-templates select="Names/Name"/></td></tr>
			<tr><td><xsl:apply-templates select="Employers"/></td></tr>
			<tr><td><xsl:apply-templates select="//HighRiskFraudAlert/Messages"/></td></tr>
			<tr><td><xsl:apply-templates select="//AddOn/ScoringModel"/></td></tr>
			<tr><td><xsl:apply-templates select="CreditSummary"/></td></tr>
			<tr><td><xsl:apply-templates select="PublicRecords"/></td></tr>
			<tr><td><xsl:apply-templates select="Collections"/></td></tr>
			<tr><td><xsl:apply-templates select="Trades"/></td></tr>
			<tr><td><xsl:apply-templates select="Inquiries"/></td></tr>
			<tr><td><xsl:apply-templates select="//Message/Recommendation"/></td></tr>
		</table>
	</xsl:template>
	
	<xsl:template match="Names/Name">
		<table class="section">
			<tr class="header">
				<td>DEMOGRAPHIC INFORMATION</td>
			</tr>
			<tr>
				<td>
					<table  width="100%">
						<tr>
							<td class="label">Guarantor:</td>
							<td><xsl:value-of select="Last"/><xsl:text>, </xsl:text><xsl:value-of select="First"/></td>
							<td></td>
							<td></td>
						</tr>	
						<tr>
							<td class="label">DOB:</td>
							<td><xsl:value-of select="//PersonalInformation/DOB"/></td>
							<td class="label">Address:</td>
							<td><xsl:value-of select="//Addresses/Address/HouseNumber"/><xsl:text> </xsl:text><xsl:value-of select="//Addresses/Address/StreetName"/><br/>
								<xsl:value-of select="//Addresses/Address/City"/><xsl:text>, </xsl:text><xsl:value-of select="//Addresses/Address/State"/>
								<xsl:text> </xsl:text><xsl:value-of select="//Addresses/Address/ZipCode"/>
							</td>
						</tr>
						<tr>
							<td class="label">SSN:</td>
							<td><xsl:apply-templates select="//PersonalInformation/SSN"/></td>
							<td class="label">Phone:</td>
							<td>(<xsl:value-of select="//PhoneAppend/PhoneNumber/AreaCode"/>)<xsl:value-of select="//PhoneAppend/PhoneNumber/Number"/></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</xsl:template>

<xsl:template match="Employers">
		<table class="section" >
			<tr>
				<td class="header">EMPLOYER INFORMATION</td>
			</tr>
			<tr>
				<td cellspacing="4">
					<table class="section"  cellspacing="0" cellpadding="0">
						<tr>
							<td class="subheading">Employer-Position</td>
							<td class="subheading">Address</td>
							<td class="subheading">Income</td>
							<td class="subheading">VerifiedReported</td>
							<td class="subheading">Hire</td>
						</tr>
						
						<xsl:for-each select="Employer">
							<tr>
								<td><xsl:value-of select="Name"/></td>
								<td><xsl:value-of select="concat(Address/City, ' ',Address/State)"/></td>
								<td><xsl:value-of select="Income"/></td>
								<td><xsl:value-of select="substring(DateVerifiedOrReported,1,7)"/></td>
								<td><xsl:value-of select="NeedBA"/>&#160;</td>
							</tr>				
						</xsl:for-each>					
					</table>
				</td>
			</tr>			
		</table>
	</xsl:template>
	
	<xsl:template match="//HighRiskFraudAlert/Messages">
		<table class="section">
			<tr>
				<td class="header">HAWK ALERTS</td>
			</tr>
			<tr>
				<td>
					<table>
						<tr>
							<xsl:for-each  select="Message">			
								<tr>
									<td><xsl:value-of select="Text"/></td>
								</tr>		
							</xsl:for-each>		
						</tr>
					</table>
				</td>
			</tr>
		</table>	
	</xsl:template>
	
	<xsl:template match="//AddOn/ScoringModel">
		<table class="section">
			<tr>
				<td class="header">MODEL PROFILE</td>
			</tr>
			<tr>
				<td></td>
			</tr>
			<tr>
				<td><b>Score Model:</b> (<xsl:value-of select="ServiceCode"/>)<xsl:text> </xsl:text>TransRisk - New Account</td>
			</tr>
			<tr>
			
				<td>
					<table>
						<tr><td>Score:</td><td><xsl:value-of select="Sign"/><xsl:value-of select="Score"/></td><td></td></tr>
						<tr><td>Factors:</td><td><xsl:value-of select="FirstFactor"/></td><td></td></tr>
						<tr><td></td><td><xsl:value-of select="SecondFactor"/></td><td></td></tr>
						<tr><td></td><td><xsl:value-of select="ThirdFactor"/></td><td></td></tr>
						<tr><td></td><td><xsl:value-of select="FourthFactor"/></td><td></td></tr>
						
					</table>
				</td>
			</tr>
		
		</table>
	
	</xsl:template>
	
	<xsl:template match="PublicRecords">
	<table class="section">
			<tr>
				<td class="header">PUBLIC RECORDS</td>
			</tr>
			<tr>
				<td>
					<table cellspacing="0" cellpadding="2">
						<tr>
							<td class="subheading">SORCE<xsl:text>/</xsl:text>DOCKET</td>
							<td class="subheading">TYPE<xsl:text>/</xsl:text>PLAINTIFF<xsl:text>/</xsl:text>ATTORNEY</td>
							<td class="subheading">DATE</td>
							<td class="subheading">LIABILITY</td>
							<td class="subheading">ECOA</td>
							<td class="subheading">COURT LOC</td>
							<td class="subheading">ASSETS</td>
						</tr>
						<xsl:apply-templates select="PublicRecord"/>
					</table>
				</td>
			</tr>			
		</table>
	</xsl:template>
	
	<xsl:template match="PublicRecord">
		<tr>
			<td class="subiteml"><xsl:value-of select="IndustryCode"/><xsl:text> </xsl:text><xsl:value-of select="MemberCode"/><br/><xsl:value-of select="DocketNumber"/></td>
			<td class="subitem"><xsl:value-of select="NeedBA"/>&#160;</td>
			<td class="subitem"><xsl:value-of select="substring(DateFiled,1,7)"/></td>
			<td class="subitem"><xsl:value-of select="LiabilitiesOrAmountOwed"/></td>
			<td class="subitem"><xsl:value-of select="NeedBA"/>&#160;</td>
			<td class="subitem"><xsl:value-of select="NeedBA"/>&#160;</td>
			<td class="subitemr"><xsl:value-of select="Assets"/></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="Collections">
		<table class="section">
			<tr class="header">
				<td>COLLECTIONS</td>
			</tr>
			<tr>
				<td>
					<table width="100%">
						<xsl:for-each select="Collection">
							
						<tr>
							<td class="label">SubName:</td><td><xsl:value-of select="NeedBA"/>&#160;</td><td class="label">Opened:</td><td><xsl:value-of select="substring(DateOpened,1,7)"/></td>
							<td class="label">Placed:</td><td><xsl:value-of select="OriginalBalance"/></td><td class="label">Creditor:</td><td><xsl:value-of select="CreditorName"/></td>
						</tr>	
						<tr>
							<td class="label">Account:</td><td><xsl:value-of select="AccountNumber"/></td><td class="label">Verified:</td><td><xsl:value-of select="substring(DateVerified,1,7)"/></td>
							<td class="label">Balance:</td><td><xsl:value-of select="CurrentBalance"/></td><td></td><td></td>
						</tr>
						<tr>
							<td class="label">SubCode:</td><td><xsl:value-of select="NeedBA"/>&#160;</td><td class="label">Closed:</td><td><xsl:value-of select="substring(DateClosed,1,7)"/></td>
							<td></td><td></td><td></td><td></td>
						</tr>	
						<tr>
							<td class="label">ECOA:</td><td><xsl:value-of select="NeedBA"/>&#160;</td><td class="label">Remarks:</td><td><xsl:value-of select="RemarksCode"/></td>
							<td></td><td></td><td></td><td></td>
						</tr>
						<tr>
							<td></td><td></td><td class="label">MOP:</td><td><xsl:value-of select="CurrentMannerOfPayment"/></td>
							<td></td><td></td><td></td><td></td>
						</tr>
						<tr><td colspan="8">&#160;</td></tr>
						</xsl:for-each>
					</table>
				</td>
			</tr>
		</table>
	
	</xsl:template>
	
	<xsl:template match="Trades">
	<table class="section">
		<tr class="header">
			<td colspan="6">TRADES</td>		
		</tr>
		<tr>
			<td>
				<table width="100%">
				<xsl:for-each select="Trade">
					
					<tr>
						<td class="label">SubName:</td><td><xsl:value-of select="SubscriberName"/></td><td class="label">Opened:</td><td><xsl:value-of select="substring(DateOpened,1,10)"/></td><td class="label">High Credit:</td><td><xsl:value-of select="HighCredit"/></td>
					</tr>	
						<tr>
						<td class="label">SubCode:</td><td><xsl:value-of select="MemberCode"/></td><td class="label">Verifed:</td><td><xsl:value-of select="substring(DateVerified,1,10)"/></td><td class="label">Credit Limit:</td><td><xsl:value-of select="CreditLimit"/></td>
					</tr>	
						<tr>
						<td class="label">Account:</td><td><xsl:value-of select="AccountNumber"/></td><td class="label">Closed:</td><td><xsl:value-of select="substring(DateClosed,1,10)"/></td><td class="label">Balance:</td><td><xsl:value-of select="Balance"/></td>
					</tr>	
						<tr>
						<td class="label">ECOA:</td><td><xsl:value-of select="AccountType"/></td><td class="label">MO:</td><td><xsl:value-of select="NeedBA"/>&#160;</td><td class="label">MaxDelq:</td><td><xsl:value-of select="MaxDelinquencyAmount"/></td>
					</tr>	
						<tr>
						<td class="label">Terms:</td><td><xsl:value-of select="TermsAmountOfPayment"/></td><td></td><td></td><td class="label">Amt-MOP:</td><td><xsl:value-of select="HighCredit"/></td>
					</tr>	
						<tr>
						<td class="label">Collatrl LoanType:</td><td><xsl:value-of select="LoanType"/></td><td></td><td></td><td class="label">PayPat(01-12):</td><td>
							<xsl:choose>
								<xsl:when test="string-length(PaymentPattern)>11">
									<xsl:value-of select="substring(PaymentPattern,1,12)"/>		
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="PaymentPattern"/>		
								</xsl:otherwise>
							</xsl:choose>	
						</td>
					</tr>	
					<tr>
						<td class="label">Remarks:</td><td><xsl:value-of select="SubscriberName"/></td><td></td><td></td><td class="label">PayPat(13-24):</td><td>
							
							<xsl:if test="string-length(PaymentPattern)>12">
							<xsl:choose>
								<xsl:when test="string-length(PaymentPattern)>23">
									<xsl:value-of select="substring(PaymentPattern,13,12)"/>		
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="substring(PaymentPattern, 13, string-length(PaymentPattern) - 12)"/>		
								</xsl:otherwise>
							</xsl:choose>
							</xsl:if>
						</td>
					</tr>	
					<tr><td colspan="6"><br/></td></tr>
						
				</xsl:for-each>
				
			</table>
			</td>
		</tr>
	</table>
	</xsl:template>

<xsl:template match="CreditSummary">
		<table class="section">
			<tr>
				<td class="header">CREDIT SUMMARY</td>
			</tr>
			<tr>
			<td>
				<table>
					<tr>
						<td>
							<table>
								<tr>
									<td class="label">PR</td><td><xsl:value-of select="NumberOfPublicRecords"/></td>
									<td class="label">NEG</td><td><xsl:value-of select="NumberOfNegativeTrades"/></td>
									<td class="label">HSTNEG</td><td><xsl:value-of select="TradesWithAnyHistoricalNegativ"/></td>
									<td class="label">TRD</td><td><xsl:value-of select="NumberOfTrades"/></td>
									<td class="label">RVL</td><td><xsl:value-of select="NumberOfRevolvingAndCheckCreditTrades"/></td>
									<td class="label">NST</td><td><xsl:value-of select="NumberOfInstallmentTrades"/></td>
									<td class="label">MTG</td><td><xsl:value-of select="NumberOfMortgageTrades"/></td>
									<td class="label">OPN</td><td><xsl:value-of select="NumberOfOpenTradeAccounts"/></td>
									<td class="label">INQ</td><td><xsl:value-of select="NumberOfInquiries"/></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="9"></td>
					</tr>
					<tr>
						<td colspan="9">
							<table cellspacing="0" cellpadding="2">
								<tr>
									<td></td>
									<td class="subheading">HIGH CREDIT</td>
									<td class="subheading">CRED LIMIT</td>
									<td class="subheading">BALANCE</td>
									<td class="subheading">PAST DUE</td>
									<td class="subheading">MNTHLY PAY</td>
								</tr>
								<xsl:apply-templates select="//CreditSummaryDescription"/>
							</table>
						</td>		
					</tr>
				</table>
				</td>
			</tr>
		</table>
	</xsl:template>
	
	<xsl:template match="//CreditSummaryDescription">
		<tr>
			<td class="label">
				<xsl:choose>
					<xsl:when test="@summaryType='R'">REVOLVING:</xsl:when>
					<xsl:when test="@summaryType='I'">INSTALLMENTS:</xsl:when>
					<xsl:when test="@summaryType='O'">OPEN:</xsl:when>
					<xsl:when test="@summaryType='M'">MORTGAGE:</xsl:when>
					<xsl:when test="@summaryType='T'">TOTALS:</xsl:when>
					<xsl:otherwise>NeedBA</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="subiteml"><xsl:value-of select="HighCredit"/></td>
			<td class="subitem"><xsl:value-of select="CreditLimit"/></td>
			<td class="subitem"><xsl:value-of select="Balance"/></td>
			<td class="subitem"><xsl:value-of select="AmountPastDue"/></td>
			<td class="subitemr"><xsl:value-of select="MonthlyPayment"/></td>
		</tr>
	</xsl:template>

	<xsl:template match="Inquiries">
		<table class="section">
			<tr>
				<td class="header">INQUIRIES</td>
			</tr>
			<tr>
				<td>
					<table cellspacing="0" cellpadding="2">
						<tr>
							<td class="subheading">Date</td>
							<td class="subheading">SubCode</td>
							<td class="subheading">SubName</td>
							<td class="subheading">Type</td>
							<td class="subheading">Amount</td>
						</tr>
						<xsl:apply-templates select="Inquiry"/>
					</table>
				</td>
			</tr>			
		</table>
	</xsl:template>
	
	<xsl:template match="Inquiry">
		<tr>
			<td class="subiteml"><xsl:value-of select="substring(DateOfInquiry,1,10)"/></td>
			<td class="subitem">
					<xsl:value-of select="concat(IndustryCode,' ',MemberCode)"/>
					&#160;
			</td>
			<td class="subitem"><xsl:value-of select="SubscriberName"/></td>
			<td class="subitem"><xsl:value-of select="InquiryType"/></td>
			<td class="subitemr"><xsl:value-of select="LoanAmount"/></td>
		</tr>
	</xsl:template>
		
	<xsl:template match="//Message/Recommendation">
		<table class="section">
			<tr>
				<td class="header">RECOMMENDATION</td>
			</tr>
			<tr>
				<td>
					<table>
						<tr>
							<td colspan="2"><xsl:value-of select="Title"/></td>
						</tr>
						<xsl:for-each select="//Step">
						<tr>
							<td width="20px"><xsl:text> </xsl:text></td><td><xsl:value-of select="@id"/><xsl:text> </xsl:text><xsl:value-of select="."/></td>
						</tr>
						<tr>
							<td></td>
						</tr>
						</xsl:for-each>
					</table>			
				</td>
			</tr>
		</table>
	</xsl:template>

</xsl:stylesheet>

  