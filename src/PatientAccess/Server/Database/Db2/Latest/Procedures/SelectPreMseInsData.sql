/**********************************************************************/
/*                                                                    */
/* iSeries400    SLPRMSINDT  - STORED PROCEDURE FOR PX                */
/*                                                                    */
/*    ************************************************************    */
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */
/*    *                                                          *    */
/*    * This unpublished material is proprietary to Perot Sys.   *    */
/*    * The methods and techniques described herein are          *    */
/*    * considered trade secrets and/or confidential.            *    */
/*    * Reproduction or distribution, in whole or in part, is    *    */
/*    * forbidden except by express written permission of        *    */
/*    * Perot Systems, Inc.                                      *    */
/*    ************************************************************    */
/*                                                                    */
/**********************************************************************/
/*  Date        Programmer          Modification Description          */
/**********************************************************************/
/* 02/26/2008   Kevin Sedota        NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTPREMSEINSDATA		                  */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTPREMSEINSDATA (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SLPRMSINDT
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

	DECLARE CURSOR1 CURSOR FOR	
	SELECT	
		HSPNUMBER ,	
		ACCOUNTNUMBER ,	
		INSUREDLASTNAME ,	
		INSUREDFIRSTNAME ,	
		INSUREDMIDDLEINITIAL ,	
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
		PRIMARYLOC , --city, stat
		PRIMARYEMPZIP ,	
		PRIMARYEMPZIPEXT ,	
		PRIMARYPHONE ,	
		SECONDARYEMPSTATUS ,	
		SECONDARYEMPCODE ,	
		SECONDARYEMPNAME ,	
		SECONDARYEMPADDRESS ,	
		SECONDARYLOC , --city,
		SECONDARYEMPZIP ,	
		SECONDARYEMPZIPEXT ,	
		SECONDARYPHONE ,	
		EFFECTIVEON ,	
		APPROVEDON ,	
		-- NOPATIENTLIABILITYFLAG , --
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
		EMPLOYEESSUPERVISOR,
		AUTHCOMPANYREPFIRSTNAME ,
		AUTHCOMPANYREPLASTNAME ,
		SERVICESAUTHORIZED ,
		EFFECTIVEDATEOFAUTH , 
		EXPIRATIONDATEOFAUTH ,
		AUTHORIZATIONSTATUS ,
		AUTHORIZATIONREMARKS,
		INSURANCECOMPANYNAME		
	FROM INSURANCES I , NHMSE01P M	
	WHERE I . HSPNUMBER = P_HSP	
		AND M . MSENACCT = P_ACCOUNTNUMBER	
		AND I . ACCOUNTNUMBER = M . MSEPACCT	
		AND I . HSPNUMBER = M . MSEHSP#	
		AND ( I . PRIORITYCODE = '1' OR I . PRIORITYCODE = '2' )	
	ORDER BY PRIORITYCODE ;
	
	OPEN CURSOR1 ;	

	
	END P1 ; 