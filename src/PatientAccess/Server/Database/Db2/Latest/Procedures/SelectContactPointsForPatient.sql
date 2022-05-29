/**********************************************************************/
/*                                                                    */
/* iSeries400    SELPATCONP  - STORED PROCEDURE FOR PX                */
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
/* 09/18/2007   Gauthreaux		    Created							                */
/* 02/14/2008   Gauthreaux        Modified  						              */
/* 12/18/2008   Deepa Raju        SR54716 Implementation - Retrieve   */
/*                                County Code & Description     		  */
/* 09/17/2012   Smitha K          Read FIPSCountyCode For             */
/*									              Physical Address          				  */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTCONTACTPOINTSFORPATIENT (

        IN P_FACILITY_ID INTEGER ,
        IN P_MRN INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SELPATCONP
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

        DECLARE CURSOR1 CURSOR FOR
 
	SELECT 0				as AddressID,
	TRIM(PATDEMO.MDPADR)	as Address1,
	''						as Address2,
	TRIM(PATDEMO.MDPCIT)	as City,
	0						as StateID,
	PATDEMO.MDPSTE			as StateCode,
	TRIM(STATE.QTSTN)		as StateDescription,
	RTRIM(PATDEMO.MDPZPA) || RTRIM(PATDEMO.MDPZ4A)
							as PostalCode,
	0						as CountryID,
	''						as CountryCode,
	''						as CountryDescription,
	0						as CountyID,
    LPAD ( TRIM ( PATDEMO.MDPCCD ), 3 , '0' )  as CountyCode,                                                 
	TRIM(COUNTY.QTCNTN)		as CountyDescription,
	''						as FIPSCountyCode,
	''						as PhoneCountryCode,
	
	-- parsing into AreaCode and PhoneNumber will be done in the broker
	
	TRIM(PATDEMO.MDPACD)	as AreaCode,
	TRIM(PATDEMO.MDPPH#)	as PhoneNumber,
	TRIM(PATDEMO.MDCLPH)	as CellPhoneNumber,
	TRIM(PATDEMO.MDEMAL)	as EmailAddress,
	0						as TYPEOFADDRESSID,
	'MAILING'				as ADDRESSDESCRIPTION,
	0						as TYPEOFCONTACTPOINTID,
	'MAILING'				as CONTACTPOINTDESCRIPTION
	
	-- this is obsolete
	--ea.MDployerAddressNumber as EmployerAddressNumber
	
	FROM	HPADMDP PATDEMO		
	LEFT OUTER JOIN	HPADQTSZ STATE				
	ON	PATDEMO.MDPSTE = STATE.QTKEY 
	AND STATE.QTHSP# = 999
	LEFT OUTER JOIN	HPADQTCO COUNTY
	ON	LPAD ( TRIM ( PATDEMO.MDPCCD ), 3 , '0' ) = COUNTY.QTKEY
	AND COUNTY.QTHSP# = P_FACILITY_ID
	WHERE PATDEMO.MDHSP# = P_FACILITY_ID
	AND	PATDEMO.MDMRC# = P_MRN
	
	UNION
		
	SELECT 0				as AddressID,
	TRIM(PATDEMO.MDMADR)	as Address1,
	''						as Address2,
	TRIM(PATDEMO.MDMCIT)	as City,
	0						as StateID,
	PATDEMO.MDMSTE			as StateCode,
	TRIM(STATE.QTSTN)		as StateDescription,
	RTRIM(PATDEMO.MDMZPA) || RTRIM(PATDEMO.MDMZ4A)
							as PostalCode,
	0						as CountryID,
	''						as CountryCode,
	''						as CountryDescription,
	0						as CountyID,
	''						as CountyCode,
	''						as CountyDescription,
	TRIM ( PATDEMO.MDCNTY ) as FIPSCountyCode, 
	''						as PhoneCountryCode,
	
	-- parsing into AreaCode and PhoneNumber will be done in the broker
	
	TRIM(PATDEMO.MDMACD)	as AreaCode,
	TRIM(PATDEMO.MDMPH#)	as PhoneNumber,
    TRIM(PATDEMO.MDCLPH)	as CellPhoneNumber,
	TRIM(PATDEMO.MDEMAL)	as EmailAddress,
	0						as TYPEOFADDRESSID,
	'PHYSICAL'				as ADDRESSDESCRIPTION,
	0						as TYPEOFCONTACTPOINTID,
	'PHYSICAL'				as CONTACTPOINTDESCRIPTION
	
	-- this is obsolete
	--ea.MDployerAddressNumber as EmployerAddressNumber
    	
    	FROM	HPADMDP PATDEMO		
		LEFT OUTER JOIN	HPADQTSZ STATE				
		ON PATDEMO.MDMSTE = STATE.QTKEY 
		AND STATE.QTHSP# = 999
		WHERE	PATDEMO.MDHSP# = P_FACILITY_ID
		AND	PATDEMO.MDMRC# = P_MRN;
	
		OPEN CURSOR1 ;
	
		END P1 ;

			
			
