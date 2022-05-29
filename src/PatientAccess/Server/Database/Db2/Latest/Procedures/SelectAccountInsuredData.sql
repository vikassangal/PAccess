/**********************************************************************/
/*                                                                    */
/* iSeries400    SLACINSDT   - STORED PROCEDURE FOR PX                */
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
/* 02/26/2006   Kevin Sedota        NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTACCOUNTINSUREDDATA                   */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTACCOUNTINSUREDDATA (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC SLACINSDT
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
        PRIMARYLOC ,  --city, state
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
        APPROVEDON ,
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
        FROM INSURANCES I
        WHERE I . HSPNUMBER = P_HSP
        AND I . ACCOUNTNUMBER = P_ACCOUNTNUMBER
        AND ( PRIORITYCODE = '1' OR PRIORITYCODE = '2' )
        ORDER BY PRIORITYCODE ;


	OPEN CURSOR1 ;
	
	END P1 ; 