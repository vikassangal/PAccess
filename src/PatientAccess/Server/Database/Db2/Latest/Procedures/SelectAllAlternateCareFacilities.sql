/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLALTERNATECAREFACILITIES - SQL PROC FOR PX            */        
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
/*************I********************I***********************************/       
--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	11/19/09 01:29:52 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLALTERNATECAREFACILITIES ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SELALLALTF 
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
	TRIM ( QTNSDC ) AS DESCRIPTION , 
	QTHSP# AS FACILITYID 
	FROM HPADQTNH 
	WHERE QTHSP# = P_FACILITYID 
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION , P_FACILITYID AS FACILITYID FROM SYSIBM / SYSDUMMY1 
	ORDER BY CODE ; 
	 
	OPEN CURSOR1 ; 
END P1  ;
