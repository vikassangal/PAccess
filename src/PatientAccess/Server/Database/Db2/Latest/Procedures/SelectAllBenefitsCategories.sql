   /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLBENEFITSCATEGORIES - SQL PROC FOR PATIENT ACCESS  */      
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
/* 02/06/2008 I  Jenney Alexandria I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/ 
/* 02/08/2008 I  Jenney Alexandria I Added sorting logic              */        
/*************I********************I***********************************/   

SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLBENEFITSCATEGORIES( ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALBTCG 
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
	 
	SELECT	BENEFITCATEGORYID AS OID ,
			TRIM ( DESCRIPTION ) AS DESCRIPTION 
	FROM	BENEFITCATEGORIES 
	WHERE	(	CASE BENEFITCATEGORYID 
				WHEN 12 THEN BENEFITCATEGORYID 
				WHEN 1 THEN BENEFITCATEGORYID 
				ELSE BENEFITCATEGORYID END) = BENEFITCATEGORYID ;
  
OPEN CURSOR1 ; 
  
END P1  ;
