/**********************************************************************/
/*                                                                    */
/* iSeries400    SELHSVWTH   - STORED PROCEDURE FOR PX                */
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
/*  Date      I  Programmer   I       Modification Description        */
/**********************************************************************/
/* 2/05/2008  I SMITHA K      I	    Created                           */
/**********************************************************************/
   
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLHOSPITALSERVICESWITH ( 
	IN @P_HSP INTEGER , 
	IN @P_HSVCD VARCHAR(5) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SELHSVWTH 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	P1 : BEGIN 
  
DECLARE CURSOR1 CURSOR FOR 
		( SELECT 0 AS SERVICEID , 
		TRIM ( M.QTKEY ) AS SERVICECODE , 
		TRIM ( M.QTMSVD ) AS SERVICEDESC , 
		M . QTHSP# AS FACILITYID , 
		TRIM ( N.NHIPTR ) AS IPFLAG , 
		TRIM ( N.NHOPFG ) AS OPFLAG ,	 
		TRIM ( N.NHDYCR ) AS DAYCAREFLAG 
		FROM HPADQTMSL1 M JOIN NHMS01P N ON 
		( N.NHHSP# = M.QTHSP#		 
		AND TRIM ( M.QTKEY ) = TRIM ( N.NHKEY ) ) 
		WHERE 
		M . QTHSP# = @P_HSP 
		AND M . QTKEY = @P_HSVCD 
		AND NOT EXISTS 
			( SELECT 
			EX.NHKEY 
			FROM NHMS02P EX 
			WHERE 
			TRIM (EX.NHKEY ) = TRIM (M.QTKEY ) 
			AND EX.NHHSP# = M.QTHSP# ) 
		)		 
	ORDER BY SERVICECODE ; 
		 
	OPEN CURSOR1 ; 
  
END P1  ;
