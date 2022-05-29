/**********************************************************************/
/*                                                                    */
/* iSeries400    SLACBLDT    - STORED PROCEDURE FOR PX                */
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
/* SP Definition - PACCESS.SELECTACCTFINANCIALDATA                    */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTACCTFINANCIALDATA (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC SLACFNDT
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

 
        DECLARE CURSOR1 CURSOR FOR
        SELECT
        FIN . PMDTBL AS BILLDROPPED ,
        FIN . PMLSCD AS LASTCHARGEDATE ,
        I . HBAMCL AS INSTOTALPAYMENTSCOLLECTED ,
        I . HBMNPY AS INSNUMBEROFMONTHLYPAYMENTS ,
        I . HBPDUE AS DAYOFMONTHPAYMENT ,
        I . HBNUM1 AS TOTALAMOUNTDUE ,  --insured
        I . HBNUM2 AS DISCPLANAMOUNT ,  -- uninsured
        I . HBNUM3 AS INSUREDTOTALMONTHLYPYMTDUE ,  -- insured
        I . HBEFPT AS UNINSTOTALMONTHLYPAYMENTSDUE ,
        I . HBUFPT AS UNINSTOTALPAYMENTSCOLLECTED ,
        I . HBUCPY AS UNINSNUMBEROFMONTHLYPAYMENTS ,
        I . HBUCBL AS UNINSUREDCONTRACTBALANCE ,
        I . HBCRSC AS RESOURCELISTPROVIDED
        FROM PM0001P FIN ,
        FF0015P F
        LEFT OUTER JOIN HPADHBP I ON
        ( I . HBHSP# = F . FFHSPN
        AND I . HBACCT = FIN .PMPT#9 )
        WHERE F . FFHSPN = P_HSP
        AND FIN . PMHSPC = F . FFHSPC
        AND FIN . PMPT#9 = P_ACCOUNTNUMBER
        AND I . HBSEQ# = 1 ;  -- only pull the financial INFO fr

 	OPEN CURSOR1 ;
	
	END P1 ; 