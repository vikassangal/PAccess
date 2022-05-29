/**********************************************************************/
/* AS400 Short Name: SDPRACTSSN                                       */
/* iSeries400        SELECTDUPLICATEPREREGACCTSMATCHINGSSN            */
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
/*                                                                    */
/***********I**********I***********I***********************************/
/*  Date    I Pgmr                 I  Modification Description        */
/***********I**********I***********I***********************************/
/* 10/03/07 I SMITHA               I NEW STORED PROCEDURE             */
/***********I**********I***********I***********************************/
/* 10/25/07 I SMITHA               I Code Review Modifications        */
/* 12/10/07 I SMITHA      II Added Check for Cancelled Prereg account */
/***********I**********I***********I***********************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTDUPLICATEPREREGACCTSMATCHINGSSN (

	IN P_SSN VARCHAR(11) ,
	IN P_FACILITYID INTEGER ,
	IN P_ADMITDATE VARCHAR(11) )

	DYNAMIC RESULT SETS 1
	LANGUAGE SQL
	SPECIFIC SDPRACTSSN
	NOT DETERMINISTIC
	MODIFIES SQL DATA
	CALLED ON NULL INPUT
	
	SET OPTION DBGVIEW = *SOURCE,
	ALWBLK = *ALLREAD ,
	ALWCPYDTA = *OPTIMIZE ,
	COMMIT = *NONE ,
	DECRESULT = (31, 31, 00) ,
	DFTRDBCOL = *NONE ,
	DYNDFTCOL = *NO ,
	DYNUSRPRF = *USER ,
	SRTSEQ = *HEX

	P1 : BEGIN

	DECLARE CURSOR1 CURSOR FOR

	SELECT
	LPP . LPACCT AS ACCOUNTNUMBER ,
	LPP . LPCL01 AS CLINICCODE ,
	LPP . LPMSV MEDICALSERVICECODE ,
	DATE ( TRIM ( CHAR ( LPP . LPADT1 ) ) || '/' ||
	TRIM ( CHAR ( LPP . LPADT2 ) ) || '/' || TRIM ( LPP . LPADT4 ) ||
	RIGHT ( '0' || TRIM ( CHAR ( LPP . LPADT3 ) ) , 2 ) )
	AS ADMISSIONDATE
	FROM
	HPADLPP LPP
	LEFT JOIN HPADMDP I
	ON ( LPP . LPHSP# = I . MDHSP#
	AND LPP . LPMRC# = I . MDMRC# )
	WHERE
	LPP . LPHSP# = P_FACILITYID
	AND I . MDSSN = P_SSN
	AND LPP . LPPTYP = '0'
	AND LPP.LPPSTI <> 'G'
	AND DATE ( TRIM ( CHAR ( LPP . LPADT1 ) ) || '/' ||
	TRIM ( CHAR ( LPP . LPADT2 ) ) || '/' || TRIM ( LPP . LPADT4 ) ||
	RIGHT ( '0' || TRIM ( CHAR ( LPP . LPADT3 ) ) , 2 ) ) =
	DATE ( TRIM ( P_ADMITDATE ) ) ;

	OPEN CURSOR1 ;

	END P1  ;
                      