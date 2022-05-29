/**********************************************************************/
/*                                                                    */
/* iSeries400    SLHCWITH   - STORED PROCEDURE FOR PX               */
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
/* 2/05/2008  I JITHIN         I	    Created                   */
/**********************************************************************/  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTHOSPITALCLINICWITH ( 
	IN P_HSP INTEGER , 
	IN P_CLINICCODE CHAR(9) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLHCWITH 
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
  
		SELECT 0 AS CLINICCODEID , 
		TRIM ( QTKEY ) AS CLINICCODE , 
		TRIM ( QTCDES ) AS DESCRIPTION , 
		TRIM ( QTHSP# ) AS FACILITYID , 
		TRIM ( QTPTST ) AS PREADMITTEST , 
		TRIM ( QTSATT ) AS SITECODE 
		FROM HPADQTCL WHERE QTHSP# = P_HSP 
		AND TRIM ( QTKEY ) = P_CLINICCODE; 
	 
OPEN CURSOR1 ; 
  
END P1  ;
