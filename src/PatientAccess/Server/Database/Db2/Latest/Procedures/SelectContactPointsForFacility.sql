/**********************************************************************/
/*                                                                    */
/* iSeries400    SELFACCONP  - STORED PROCEDURE FOR PX                */
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
/* 02/14/2008   GAUTHREAUX                  MODIFIED                  */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTCONTACTPOINTSFORFACILITY (

        IN P_FACILITY_ID INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SELFACCONP
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN
				
        DECLARE CURSOR1 CURSOR FOR

	SELECT 0		as AddressID,
	TRIM(FAC.FFFMAD)	as Address1,
	''			as Address2,
	TRIM(FAC.FFFMCT)	as City,
	0			as StateID,
	FAC.FFFMST		as StateCode,
	TRIM(STATE.QTSTN)	as StateDescription,
	FAC.FFFMZP		as PostalCode,
	0			as CountryID,
	''			as CountryCode,
	''			as CountryDescription,
	0			as CountyID,
	''			as CountyCode,
	''			as CountyDescription,
	''			as PhoneCountryCode,
	
	-- parsing into AreaCode and PhoneNumber will be done in the broker
	
	--pn.AreaCode		as AreaCode,
	--pn.PhoneNumber	as PhoneNumber,
	FAC.FFFMTL		as AreaCodeAndPhone,
	
	''			as EmailAddress,
	0			as TYPEOFADDRESSID,
	'PHYSICAL'		as ADDRESSDESCRIPTION,
	0			as TYPEOFCONTACTPOINTID,
	'PHYSICAL'		as CONTACTPOINTDESCRIPTION
	
	-- this is obsolete
	--ea.EmployerAddressNumber as EmployerAddressNumber
        	
        	FROM	FF0015P FAC
			LEFT OUTER JOIN	HPADQTSZ STATE				
			ON FAC.FFFMST = STATE.QTKEY
			AND STATE.QTHSP# = 999
			WHERE	FAC.FFHSPN = P_FACILITY_ID;
	
	OPEN CURSOR1 ;
	
	END P1 ;

							