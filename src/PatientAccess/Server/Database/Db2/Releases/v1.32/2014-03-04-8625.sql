/**********************************************************************/
/*                                                                    */
/* iSeries400    SELGARCONP  - STORED PROCEDURE FOR PX                */
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
/* 09/18/2007   Gauthreaux		    Created                   */
/* 02/14/2008   Gauthreaux                  Modified                  */
/* 03/04/2014   Srilakshmi			Select MGCLRK as Guarantor's  */
/*													CellPhoneConsent value           */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTCONTACTPOINTSFORGUARANTOR (

        IN P_FACILITY_ID INTEGER ,
        IN P_ACCOUNT_NUMBER INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SELGARCONP
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

        DECLARE CURSOR1 CURSOR FOR

	SELECT 0		as AddressID,
	TRIM(GUAR.MGGAD1)	as Address1,
	TRIM(GUAR.MGGAD2)	as Address2,
	TRIM(GUAR.MGGCIT)	as City,
	0			as StateID,
	GUAR.MGGSTE		as StateCode,
	TRIM(STATE.QTSTN)	as StateDescription,
	RTRIM(GUAR.MGGZPA) || RTRIM(GUAR.MGGZ4A)
				as PostalCode,
	0			as CountryID,
	''			as CountryCode,
	''			as CountryDescription,
	0			as CountyID,
	''			as CountyCode,
	TRIM(GUAR.MGGCNT)	as CountyDescription,
	''			as PhoneCountryCode,
	
	GUAR.MGGACD				as AreaCode,				
	GUAR.MGGPH#				as PhoneNumber,
	GUAR.MGCLPH				as CellPhoneNumber,
	TRIM(GUAR.MGEMAL)		as EmailAddress,
	TRIM(GUAR.MGCLRK)   as CellPhoneConsent,
	
	0			as TYPEOFADDRESSID,
	'MAILING'		as ADDRESSDESCRIPTION,
	0			as TYPEOFCONTACTPOINTID,
	'MAILING'		as CONTACTPOINTDESCRIPTION
	
	-- this is obsolete
	--ea.EmployerAddressNumber as EmployerAddressNumber
        	
        	FROM	HPADMGP GUAR			
			LEFT OUTER JOIN	HPADQTSZ STATE
			ON GUAR.MGGSTE = STATE.QTKEY 
			AND STATE.QTHSP# = 999
			WHERE	GUAR.MGHSP# = P_FACILITY_ID 
			AND GUAR.MGGAR# = P_ACCOUNT_NUMBER;
	
	OPEN CURSOR1 ;
	
	END P1 ;

			