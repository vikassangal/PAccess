/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTRESEARCHSTUDYWITH - SQL PROC                   */        
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
/* 11/10/2009 I Smitha			   I NEW STORED PROCEDURE             */        
/* 01/12/2010 I Deepa Raju		   I Modified Stored procedure name   */        
/*			  I					   I  from SELECTRESEARCHSPONSORMWITH */        
/*			  I					   I  to SELECTRESEARCHSTUDYWITH	  */        
/*************I********************I***********************************/        
--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	10/15/09 01:32:26 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTRESEARCHSTUDYWITH ( 
	IN P_FACILITYID INTEGER , 
	IN P_CODE VARCHAR(10) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC  SLRSTDYWTH 
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
	TRIM ( QTRSDC ) AS DESCRIPTION , 
	QTHSP# AS FACILITYID 
	FROM HPADQTRZ 
	WHERE QTHSP# = P_FACILITYID 
	AND QTKEY = P_CODE ; 
  
OPEN CURSOR1 ; 
  
END P1  ;
