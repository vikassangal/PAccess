/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETPCPFOR - SQL PROC FOR PX            */        
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
/* 09/06/2012 I Deepak			   I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/       
--  Generate SQL 
--  Version:                   	V6R1M0 080215 
--  Generated on:              	09/06/12 08:24:26 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETPCPFOR ( 
	IN P_HSP INTEGER , 
	IN P_MRC INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC GETPCPFOR 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	CLOSQLCSR = *ENDMOD , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	P1 : BEGIN 
	DECLARE CURSOR1 CURSOR FOR 
	SELECT V . OTHERDRID 
	FROM PACCESS . ACCOUNTPROXIES V 
	LEFT JOIN HPDATA1 . HPADHBP E ON 
	( V . FACILITYID = E . HBHSP# 
	AND V . MEDICALRECORDNUMBER = E . HBMRC# 
	AND V . ACCOUNTNUMBER = E . HBACCT 
	AND E . HBSEQ# = 1 ) 
	WHERE V . FACILITYID = P_HSP 
	AND V . MEDICALRECORDNUMBER = P_MRC 
	AND V . ACCOUNTNUMBER = P_ACCOUNTNUMBER; 
	OPEN CURSOR1 ; 
	END P1  ;

