/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLPLACESOFWORSHIPWITH                         */
/*                             - SQL PROC FOR PATIENT ACCESS          */      
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
/* 01/15/2008 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/ 
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLPLACESOFWORSHIPWITH ( 
	IN P_FACILITYID INTEGER , 
	IN P_PLACEOFWORSHIPCODE VARCHAR(10) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALPLWRWI 
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
	TRIM ( QTPARD ) AS DESCRIPTION, 
	QTHSP# AS FACILITYID 
	FROM HPADQTPZ 
	WHERE QTHSP# = P_FACILITYID 
	AND TRIM ( QTKEY ) = TRIM ( P_PLACEOFWORSHIPCODE ) ; 
	 
	OPEN CURSOR1 ; 
END P1  ;
