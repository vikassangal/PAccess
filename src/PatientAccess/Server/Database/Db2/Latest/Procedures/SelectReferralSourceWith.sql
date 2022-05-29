/**********************************************************************/
/*                                                                    */
/* iSeries400  SELECTREFERRALSOURCEWITH - STORED PROCEDURE FOR PX     */
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
/* 02/14/2008 I KIRAN         I	    NEW STORED PROCEDURE              */
/**********************************************************************/
/* 02/21/2008 I  Kiran		I Removed usage of Trim() in where clause */
/*************I********************I***********************************/
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTREFERRALSOURCEWITH ( 
	IN P_CODE VARCHAR(6) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLREFSRCWT 
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
	SELECT 0 AS OID , 
		TRIM ( RSCD ) AS CODE , 
		TRIM ( RSDESC ) AS DESCRIPTION 
		FROM NHADRSP 
	WHERE RSCD = P_CODE;
  
OPEN CURSOR1 ; 
  
END P1  ;
 