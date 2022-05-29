﻿--  Generate SQL 
--  Version:                   	V7R2M0 140418 
--  Generated on:              	09/24/17 21:12:01 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
SET PATH *LIBL ; 
  
CREATE PROCEDURE PACCESS.SELECTPREMSEINSDATA ( 
	IN P_HSP INTEGER , 
	IN P_MRN INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER ) 
	DYNAMIC RESULT SETS 10 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SLPRMSINDT 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	CLOSQLCSR = *ENDACTGRP , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DLYPRP = *NO , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	RDBCNNMTH = *RUW , 
	SRTSEQ = *HEX   
	P1 : BEGIN	DECLARE CURSOR1 CURSOR FOR		
	SELECT			HSPNUMBER ,			
	ACCOUNTNUMBER ,			
	INSUREDLASTNAME ,			
	INSUREDFIRSTNAME ,			
	INSUREDMIDDLEINITIAL ,	
	INSUREDNAMESUFFIX	,
	INSUREDSEX ,			
	INSUREDRELATIONSHIP ,			
	INSUREDIDENTIFIER ,			
	INSUREDCOUNTRYCODE ,			
	INSUREDBIRTHDATE ,			
	INSURANCECERTIFICATIONNUM ,			
	INSURANCEGROUPNUMBER ,			
	GROUPNUMBER ,			
	POLICYNUMBER ,			
	EVCNUMBER ,			
	AUTHORIZATIONNUMBER ,			
	AUTHORIZEDDAYS ,			
	ISSUEDATE ,			
	INSUREDADDRESS ,			
	INSUREDCITY ,			
	INSUREDSTATE ,			
	INSUREDZIP ,			
	INSUREDZIPEXT ,			
	INSUREDAREACODE1 ,			
	INSUREDPHONENUMBER1 ,			
	INSUREDAREACODE2 ,			
	INSUREDPHONENUMBER2 ,			
	INSUREDCELLPHONE ,			
	INSURANCECOMPANYNUMBER ,		
	SIGNEDOVERMEDICAREHICNUMBER ,			
	PRIORITYCODE ,			
	BILLINGADDRESS1 ,			
	BILLINGCITYSTATECOUNTRY ,			
	BILLINGZIPZIPEXTPHONE ,			
	DEDUCTIBLE ,			
	COPAY ,			
	NOLIABILITY ,			
	ELIGIBILITY ,			
	CONDITIONOFSERVICE ,			
	PRIMARYEMPSTATUS ,			
	PRIMARYEMPCODE ,			
	PRIMARYEMPNAME ,			
	PRIMARYEMPADDRESS ,			
	PRIMARYLOC ,  --city, stat   
	                                              
	PRIMARYEMPZIP ,			
	PRIMARYEMPZIPEXT ,			
	PRIMARYPHONE ,			
	SECONDARYEMPSTATUS ,			
	SECONDARYEMPCODE ,			
	SECONDARYEMPNAME ,			
	SECONDARYEMPADDRESS ,			
	SECONDARYLOC ,  --city,                                                        
		SECONDARYEMPZIP ,			
		SECONDARYEMPZIPEXT ,			
		SECONDARYPHONE ,			
		EFFECTIVEON ,			
		APPROVEDON ,			 -- NOPATIENTLIABILITYFLAG , --  
		                                              
		AUTHORIZATIONPHONE ,			
		AUTHORIZATIONCOMPANY ,			
		AUTHORIZATIONPMTEXT ,			
		AUTHORIZATIONFLAG ,			
		VERIFICATIONDATE ,			
		VERIFICATIONFLAG ,			
		VERIFIEDBY ,			
		ATTORNEYNAME ,			
		ATTORNEYSTREET ,			
		ATTORNEYCITY ,			
		ATTORNEYSTATE ,			
		ATTORNEYZIP5 ,			
		ATTORNEYZIP4 ,			
		ATTORNEYCOUNTRYCODE ,			
		ATTORNEYPHONE ,			
		AGENTNAME ,			
		AGENTPHONE ,			
		AGENTSTREET ,			
		AGENTCITY ,			
		AGENTSTATE ,			
		AGENTZIP5 ,			
		AGENTZIP4 ,			
		AGENTCOUNTRYCODE ,			
		BILLINGCAREOFNAME ,			
		BILLINGNAME ,			
		TRACKINGNUMBER ,			
		ADJUSTERSNAME ,			
		EMPLOYEESSUPERVISOR ,		
		AUTHCOMPANYREPFIRSTNAME ,		
		AUTHCOMPANYREPLASTNAME ,		
		SERVICESAUTHORIZED ,		
		EFFECTIVEDATEOFAUTH ,		
		EXPIRATIONDATEOFAUTH ,		
		AUTHORIZATIONSTATUS ,		
		AUTHORIZATIONREMARKS ,		
		INSURANCECOMPANYNAME			
		FROM INSURANCES I , NHMSE01P M		
		WHERE I . HSPNUMBER = P_HSP			AND 
		M . MSENACCT = P_ACCOUNTNUMBER			AND 
	    I . ACCOUNTNUMBER = M . MSEPACCT			AND 
		I . HSPNUMBER = M . MSEHSP#			AND 
		( I . PRIORITYCODE = '1' OR I . PRIORITYCODE = '2' )		
		ORDER BY PRIORITYCODE ;		
		OPEN CURSOR1 ;		
		END P1  ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE PACCESS.SLPRMSINDT 
TO HPPGMR ;
