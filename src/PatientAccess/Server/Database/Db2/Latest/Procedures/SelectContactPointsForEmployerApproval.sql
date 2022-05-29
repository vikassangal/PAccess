 			
/**********************************************************************/
/*                                                                    */
/* iSeries400    SELEMPCONP  - STORED PROCEDURE FOR PX                */
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
/* 09/18/2007   					Created                           */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTCONTACTPOINTSFOREMPLOYERAPPROVAL (

        IN P_FACILITY_ID INTEGER ,
        IN P_EMP_CODE INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SELEMPCPFA
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

        DECLARE CURSOR1 CURSOR FOR

			SELECT 0					as AddressID,
			TRIM(EMPADDR.EMADDR)		as Address1,
			''							as Address2,
			TRIM(EMPADDR.EMCITY)		as City,
			0							as StateID,
			EMPADDR.EMST				as StateCode,
			TRIM(STATE.QTSTN)			as StateDescription,
			EMPADDR.EMZIP				as PostalCode,
			0							as CountryID,
			''							as CountryCode,
			''							as CountryDescription,
			0							as CountyID,
			''							as CountyCode,
			''							as CountyDescription,
			''							as PhoneCountryCode,
			EMPADDR.EMPH#				as AreaCodeAndPhone,		
			''							as EmailAddress,
			0							as TYPEOFADDRESSID,
			'EMPLOYER'					as ADDRESSDESCRIPTION,
			0							as TYPEOFCONTACTPOINTID,
			'EMPLOYER'					as CONTACTPOINTDESCRIPTION
			
        	FROM	EMPLOYERSFORAPPROVAL EMP
        	JOIN	FF0015P FAC										ON FAC.FFHSPN = P_FACILITY_ID			
			LEFT OUTER JOIN	EMPLOYERADDRESSFORAPPROVAL EMPADDR		ON EMP.EMCODE = EMPADDR.EMCODE 
																	AND EMP.EMNAME = EMPADDR.EMANAM
																	AND EMP.EMFUUN = FAC. FFFUUN 
																	AND EMP.EMFUUN = EMPADDR.EMFUUN
			
			LEFT OUTER JOIN	HPADQTSZ STATE							ON EMPADDR.EMST = STATE.QTKEY 
																	AND STATE.QTHSP# = 999
																	
			WHERE	EMP.EMCODE = P_EMP_CODE;
	
	OPEN CURSOR1 ;
	
	END P1 ;

			