 /**********************************************************************/
/*                                                                    */
/* iSeries400    SLACPRMSE   - STORED PROCEDURE FOR PX                */
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
/* 02/26/2008   Kevin Sedota        NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTACCTPREMEACCT		                  */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTACCTPREMEACCT (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC SLACPRMSE
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT

        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

	DECLARE CURSOR1 CURSOR FOR
	SELECT	M . MSEPACCT AS COPIEDFROMACCOUNTNUMBER ,	
		A . LPFC AS FinancialCode,	
		B . LPIPA AS IPACODE ,	
		B . LPIPAC AS IPACLINICCODE	
	FROM	NHMSE01P M 	
	join HPADLPP A on (A . LPHSP# = M . MSEHSP#		
		AND A . LPMRC# = M . MSEMRC#		
		AND A . LPACCT = M . MSEPACCT)	
	join HPADLPP B on ( A . LPHSP# = B . LPHSP#		
		AND A . LPMRC# = B . LPMRC#		
		AND A . LPACCT = B . LPACCT)	
	WHERE	M . MSEHSP# = P_HSP 		
	AND M . MSEMRC# = P_MRN 		
	AND M . MSENACCT = P_ACCOUNTNUMBER; 


	OPEN CURSOR1 ;
	
	END P1 ;