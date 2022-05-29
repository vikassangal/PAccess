 /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLNPPVERSIONFOR - SQL PROC FOR PATIENT ACCESS */
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
/* 01/28/2008 I  Kiran P Kumar     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/ 
/* 02/21/2008 I  Kiran		       I Removed usage of temp table      */        
/* 06/13/2008 I  Deepa Raju	       I OTD# 37577 fix - Added DISTINCT  */        
/*************I********************I***********************************/
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLNPPVERSIONFOR ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALLNPPFR 
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
	TRIM ( QTNPPD ) AS DESCRIPTION, 
	QTHSP# AS FACILITYID 
	FROM HPADQTNP 
	WHERE QTHSP# = P_FACILITYID 
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION , P_FACILITYID AS FACILITYID FROM SYSIBM / SYSDUMMY1
	ORDER BY CODE ; 
	 
	OPEN CURSOR1 ; 
END P1  ;
 