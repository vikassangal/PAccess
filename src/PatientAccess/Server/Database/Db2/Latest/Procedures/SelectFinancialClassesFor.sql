/**********************************************************************/
/*                                                                    */
/* iSeries400    SLFNCLFOR   - STORED PROCEDURE FOR PX                */
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
/* 2/04/2008  I JITHIN        I	    Created                           */
/**********************************************************************/
  
SET PATH *LIBL ; 
CREATE PROCEDURE SELECTFINANCIALCLASSESFOR ( 
	IN P_FINCLASSTYPEID VARCHAR(10) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLFNCLFOR 
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
  
  
	SELECT DISTINCT 0 AS OID , 
	TRIM ( QTKEY ) AS CODE , 
	TRIM ( QTFCD ) AS DESCRIPTION 
	FROM HPADQTFC 
	WHERE ( ( P_FINCLASSTYPEID <> '1' OR TRIM ( QTKEY ) NOT IN ( '66' , '70' , '72' , '73' , '96' ) ) 
	AND ( P_FINCLASSTYPEID <> '2' OR  TRIM ( QTKEY ) IN ( '66' , '70' , '72' , '73' , '96' ) ) )
	AND QTHSP# = 999 ; 
  
	OPEN CURSOR1 ; 
  
END P1  ;
 