/**********************************************************************/
/*                                                                    */
/* iSeries400    SELPATECP  - STORED PROCEDURE FOR PX                 */
/*                                                                    */
/*    ************************************************************    */
/*    * Perot SystLAWs, Copyright 2003, All rights reserved(U.S.) *   */
/*    *                                                          *    */
/*    * This unpublished material is proprietary to Perot Sys.   *    */
/*    * The methods and techniques described herein are          *    */
/*    * considered trade secrets and/or confidential.            *    */
/*    * Reproduction or distribution, in whole or in part, is    *    */
/*    * forbidden except by express written permission of        *    */
/*    * Perot SystLAWs, Inc.                                      *   */
/*    ************************************************************    */
/*                                                                    */
/**********************************************************************/
/*  Date        Programmer          Modification Description          */
/**********************************************************************/
/* 09/22/2007   Gauthreaux		    Created                   */
/* 02/14/2008   Gauthreaux                  Modified                  */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTCONTACTPOINTSFORPATEMPLOYER (
        IN P_FACILITY_ID INTEGER ,
        IN P_MRN INTEGER )
        
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SELPATECP
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

        DECLARE CURSOR1 CURSOR FOR

	SELECT 0			as AddressID,
	TRIM(EMPADDR.LAWADR)		as Address1,
	''				as Address2,
	TRIM(EMPADDR.LAWCIT)		as City,
	0				as StateID,
	EMPADDR.LAWSTE			as StateCode,
	TRIM(STATE.QTSTN)		as StateDescription,
	RTRIM(EMPADDR.LAWZPA) || RTRIM(EMPADDR.LAWZ4A)
					as PostalCode,
	0				as CountryID,
	''				as CountryCode,
	''				as CountryDescription,
	0				as CountyID,
	''				as CountyCode,
	''				as CountyDescription,
	''				as PhoneCountryCode,
	
	-- parsing into AreaCode and PhoneNumber will be done in the broker
	
	--pn.AreaCode			as AreaCode,
	--pn.PhoneNumber		as PhoneNumber,
	EMPADDR.LAWPH#			as AreaCodeAndPhone,
	
	''				as EmailAddress,
	0				as TYPEOFADDRESSID,
	'EMPLOYER'			as ADDRESSDESCRIPTION,
	0				as TYPEOFCONTACTPOINTID,
	'EMPLOYER'			as CONTACTPOINTDESCRIPTION
	
	-- this is obsolete
	--ea.LAWployerAddressNumber as LAWployerAddressNumber
        	
        	FROM	HPADLAP5 EMPADDR	
        	LEFT OUTER JOIN	HPADQTSZ STATE		
        	ON EMPADDR.LAWSTE = STATE.QTKEY 
        	AND STATE.QTHSP# = 999			
		WHERE	EMPADDR.LAHSP# = P_FACILITY_ID 
		AND	EMPADDR.LAMRC# = P_MRN;
	
	OPEN CURSOR1 ;
	
	END P1 ;

			