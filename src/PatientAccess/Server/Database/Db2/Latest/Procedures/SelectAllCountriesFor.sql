/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLCOUNTRIESFOR - SQL PROC FOR PX        */        
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
/* 01/15/2009 I  Smitha Krishnamurthy     I  NEW STORED PROCEDURE     */        
/*************I********************I***********************************/        

SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLCOUNTRIESFOR ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SELALCRTYF
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	DBGVIEW = *SOURCE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	P1 : BEGIN 
  
	DECLARE CURSOR1 CURSOR FOR 
	SELECT OID , CODE , DESCRIPTION , FACILITYID 
	 
	FROM 
	( SELECT 0 AS OID , 
	TRIM ( QTKEY ) AS CODE , 
	TRIM ( QTCNTD ) AS DESCRIPTION , 
	QTHSP# AS FACILITYID 
	FROM HPADQTCY 
	WHERE QTHSP# = P_FACILITYID 
	AND	QTKEY IS NOT NULL		 
	AND	QTKEY <> ''	 
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION , 
	P_FACILITYID AS FACILITYID FROM SYSIBM / SYSDUMMY1 ) 
	AS V ( OID , CODE , DESCRIPTION , FACILITYID ) 
	ORDER BY 
	CASE  
		WHEN CODE = ' '		THEN	1 
		WHEN CODE = 'USA'	THEN	2 
		WHEN CODE = 'CAN'	THEN	3 
		WHEN CODE = 'MEX'	THEN	4 

		END,
		CASE
		WHEN ( CODE <> ' ' 
			AND CODE <> 'USA'
			AND CODE <> 'CAN'
			AND CODE <> 'MEX' )	THEN DESCRIPTION
			END ; 
	OPEN CURSOR1 ; 
END P1  ;
