/**********************************************************************/
/*                                                                    */
/* iSeries400    SLDMGINFO   - STORED PROCEDURE FOR PX               */
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
  
CREATE PROCEDURE SELECTDEMOGRAPHICSINFO ( 
	IN P_MRN INTEGER , 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLDMGINFO 
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
	SELECT MDP . MDMAR AS MARITALSTATUSCODE, 
	MDP . MDRACE AS RACECODE , 
	MDP . MDETHC AS ETHNICITYCODE , 
	MDP . MDPTID AS NATIONALID , 
	MDP . MDDL# AS DRIVINGLICENSE , 
	MDP . MDLNGC AS LANGUAGECODE , 
	MDP . MDPOB AS PLACEOFBIRTH , 
	MDP . MDRLGN AS RELIGIONCODE , 
	MDP . MDPARC AS RELIGIOUSCONGREGATIONCODE
	FROM HPADMDU MDP 
	WHERE MDMRC# = P_MRN 
	AND MDHSP# = P_FACILITYID ; 
  
  
  
OPEN CURSOR1 ; 
  
END P1  ;
