/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETCPTCODESFOR - SQL PROC             */        
/*            I                                                       */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *       I                                                  *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods and techniques described herein are          *    */        
/*    * considered trade secrets and/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*            I                                                       */        
/*************I********************I***********************************/        
/*  Date      I  Programmer        I  Modification Description        */        
/*************I********************I***********************************/        
/* 10/01/2014 I Srilakshmi   I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/   
  
CREATE PROCEDURE GETCPTCODESFOR ( 
	IN P_HSPCODE INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER , 
	IN P_MRN INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC GETCPTCODE 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
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
				SELECT 
					LPHIMSEQ# AS CPTSEQUENCE , 
					LPHIMHCPC AS CPTCODE 
				FROM 
					HPHCPCLPP CPTCODES 
				WHERE 
					CPTCODES . LPHIMHSP# = P_HSPCODE 
				AND CPTCODES . LPHIMACCT = P_ACCOUNTNUMBER 
				AND CPTCODES . LPHIMMRC# = P_MRN 
				ORDER BY LPHIMSEQ# ASC ; 
				 
OPEN CURSOR1 ; 
END P1  ;
