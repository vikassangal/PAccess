/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETMOSTRECENTACCDETAILSFORAPATIENT - SQL PROC FOR PX            */        
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
--  Generated on:              	09/06/12 08:18:52 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETMOSTRECENTACCDETAILSFORAPATIENT ( 
	IN P_HSP INTEGER , 
	IN P_HSPCODE CHAR(3) , 
	IN P_MRC INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC GETMOSTRECAC 
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
	WITH RECENTACCOUNTDETAILS AS 
	( 
	SELECT V . ACCOUNTNUMBER , FUS . ACENTD , FUS . ACENTT , ROW_NUMBER ( ) OVER ( PARTITION BY FUS . ACPT#9 ORDER BY FUS . ACENTD , FUS . ACENTT ASC ) AS ROWNUM 
  
	FROM PADATA . AC0005P FUS INNER JOIN PACCESS . ACCOUNTPROXIES V ON FUS . ACPT#9 = V . ACCOUNTNUMBER 
  
	WHERE 
  
	FUS . ACHSPC = P_HSPCODE AND FUS . ACACTC = 'NWACT' AND V . MEDICALRECORDNUMBER = P_MRC 
	) 
	SELECT ACCOUNTNUMBER , ACENTD FROM RECENTACCOUNTDETAILS WHERE ROWNUM = 1 ORDER BY ACENTD, ACENTT DESC FETCH FIRST ROW ONLY ; 
	OPEN CURSOR1 ; 
	END P1  ;

