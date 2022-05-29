
/*                                                                    */
/* iSeries400    SLALRELNS   - STORED PROCEDURE FOR PX               */
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
CREATE PROCEDURE SELECTALLRELATIONSHIPS (
	P_TypesOfRole INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALRELNS 
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
		TRIM ( QTKEY ) AS CODE , 
		TRIM ( QTRED ) AS DESCRIPTION 
		FROM HPADQTRE 
		WHERE QTHSP# = 999 
		AND P_TypesOfRole = 0
	UNION ALL 
		SELECT DISTINCT 0 AS OID , 
		TRIM (R.QTKEY ) AS CODE , 
		TRIM ( R.QTRED ) AS DESCRIPTION 
		FROM HPADQTRE R,VALIDRELATIONSHIPCONTEXTS V
		WHERE TRIM(UPPER(R.QTKEY)) = TRIM(UPPER(V.RELATIONSHIPCODE))
		AND R.QTHSP# = 999 
		AND P_TypesOfRole <> 0
		AND V.TYPEOFROLEID = P_TypesOfRole
   UNION ALL 
		SELECT DISTINCT 0 AS OID ,
		'' AS CODE ,
		'' AS DESCRIPTION 
		FROM VALIDRELATIONSHIPCONTEXTS
		WHERE TYPEOFROLEID = P_TypesOfRole OR P_TypesOfRole = 0
	ORDER BY CODE ; 

  

OPEN CURSOR1 ; 

  

END P1  ;
 