/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLMODEOFARRIVALFOR -                        */
/*                               SQL PROC FOR PATIENT ACCESS          */      
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
/* 01/16/2008 I  Kiran P Kumar     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/ 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLMODEOFARRIVALFOR ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALLMOAFR 
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
	SELECT OID , CODE , DESCRIPTION, FACILITYID 
	FROM 
	( SELECT 0 AS OID , 
	TRIM ( NDKEY ) AS CODE , 
	TRIM ( NDDES ) AS DESCRIPTION, 
	NDHSP# AS FACILITYID 
	FROM NDAV01P 
	WHERE NDHSP# = P_FACILITYID 
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION , P_FACILITYID AS FACILITYID FROM SYSIBM / SYSDUMMY1 ) 
		AS V ( OID , CODE , DESCRIPTION , FACILITYID ) 
	ORDER BY CODE ; 
	 
	OPEN CURSOR1 ; 
END P1  ;
 