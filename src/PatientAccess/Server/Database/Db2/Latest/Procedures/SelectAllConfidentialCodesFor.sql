 /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SelectAllConfidentialCodesFor -                      */
/*                                       SQL PROC FOR PATIENT ACCESS  */      
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
/* 01/16/2008 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/   
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLCONFIDENTIALCODESFOR ( 
IN P_HSP INTEGER  )
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL
	SPECIFIC SLALLCONCD
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

SELECT OID,CODE,DESCRIPTION
FROM
    (SELECT 0 AS OID , 
	TRIM ( NDKEY ) AS CODE , 
	TRIM ( NDCFDS ) AS DESCRIPTION 
	FROM NDCN01P 
	WHERE NDHSP# =  P_HSP 
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION FROM SYSIBM / SYSDUMMY1 ) 
		AS V ( OID , CODE , DESCRIPTION )
	order by CODE; 

OPEN CURSOR1 ; 

END P1  ;

 