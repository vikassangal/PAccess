  /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTCATEGORIESFOR - SQL PROC FOR PATIENT ACCESS  */      
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
/* 02/06/2008 I  Jenney Alexandria      I NEW STORED PROCEDURE        */        
/*************I********************I***********************************/   
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTCATEGORIESFOR( 
	IN P_CODE VARCHAR(10) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLCGFRSRCD 
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
		SELECT BC.BENEFITCATEGORYID AS BENEFITCATEGORYID, 
			   TRIM ( BC.DESCRIPTION ) AS DESCRIPTION 
		FROM HSVBENEFITCATEGORYXREF X 
		LEFT OUTER JOIN BENEFITCATEGORIES BC 
		ON ( BC.BENEFITCATEGORYID = X.BENEFITCATEGORYID ) 
		LEFT OUTER JOIN HPADQTMSL1 M 
		ON ( M.QTKEY = X.SERVICECODE ) 
		WHERE M.QTHSP# = 999
		AND M.QTKEY = P_CODE;
  
OPEN CURSOR1 ; 
  
END P1 ;
