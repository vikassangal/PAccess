/***********************************************************************/
/*                                                                     */
/* iSeries400    SELHSVFOR   - STORED PROCEDURE FOR PX                 */
/*                                                                     */
/*    ************************************************************     */
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *     */
/*    *                                                          *     */
/*    * This unpublished material is proprietary to Perot Sys.   *     */
/*    * The methods and techniques described herein are          *     */
/*    * considered trade secrets and/or confidential.            *     */
/*    * Reproduction or distribution, in whole or in part, is    *     */
/*    * forbidden except by express written permission of        *     */
/*    * Perot Systems, Inc.                                      *     */
/*    ************************************************************     */
/*                                                                     */
/***********************************************************************/
/*  Date      I  Programmer I         Modification Description         */
/***********************************************************************/
/* 2/05/2008  I SMITHA K    I	    Created                            */
/***********************************************************************/
/* 02/21/2008 I Kiran		I Removed usage of Trim() in where clause  */        
/* 06/02/2008 I Deepa Raju	I OTD# 37483-Added DISTINCT to SELECT stmt */        
/*************I********************I************************************/
 
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLHOSPITALSERVICESFOR ( 

	IN @P_HSP INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SELHSVFOR 
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
		( SELECT DISTINCT
		0 AS SERVICEID , 
		TRIM ( M.QTKEY ) AS SERVICECODE , 
		TRIM ( M.QTMSVD ) AS SERVICEDESC , 
		M . QTHSP# AS FACILITYID , 
		TRIM ( N.NHIPTR ) AS IPFLAG , 
		TRIM ( N.NHOPFG ) AS OPFLAG ,	 
		TRIM ( N.NHDYCR ) AS DAYCAREFLAG 
		FROM HPADQTMSL1 M JOIN NHMS01P N ON 
		( N.NHHSP# = M . QTHSP# 
		AND M.QTKEY = N.NHKEY ) 
		WHERE 
		M . QTHSP# = @P_HSP 

		AND NOT EXISTS ( 
			SELECT EX . NHKEY 
			FROM NHMS02P EX 
			WHERE 
			EX.NHKEY = M.QTKEY
			AND EX.NHHSP# = M.QTHSP# ) 

		UNION ALL 

			SELECT 
			0 AS SERVICEID , 
			'' AS SERVICECODE , 
			'' AS DESCRIPTION , 
			0 AS FACILITYID , 
			'N' AS IPFLAG , 
			'Y' AS OPFLAG , 
			'N' AS DAYCAREFLAG 
			FROM SYSIBM / SYSDUMMY1) 	 

	ORDER BY SERVICECODE ; 
	OPEN CURSOR1 ; 

END P1  ;
 