/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETIPAFOR - SQL PROC FOR PX                          */        
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
/* 05/13/2013 I  Srilakshmi 	   I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/       
--  Generate SQL 
--  Version:                   	V7R1M0 100423 
--  Generated on:              	05/12/13 23:42:12 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETIPAFORPATIENT ( 
	IN MD_HSP INTEGER , 
	IN MD_MRC INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC GETIPAFOR 
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
	SELECT A . MDIPA AS PATIENTIPACODE , 
	A . MDIPAC AS PATIENTIPACLINICCODE 
	FROM HPDATA1 . HPADMDP A 
	WHERE A . MDHSP# = MD_HSP 
	AND A . MDMRC# = MD_MRC ; 
	OPEN CURSOR1 ; 
	END P1  ; 
  


