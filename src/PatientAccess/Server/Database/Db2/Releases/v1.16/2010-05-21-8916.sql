/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GetSelectableConditionCodes -                        */
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
/* This stored procedure returns all the Condition Codes              */
/* that are approved for and selectable in Patient Access,            */
/* from the Condition Code table HPDATA1.HPADQTCC.                    */
/*                                                                    */
/*************I********************I***********************************/        
/*  Date      I  Programmer        I  Modification Description        */        
/*************I********************I***********************************/        
/* 01/16/2008 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/* 05/21/2010 I  Deepa Raju        I SR804- Add new Condition Code P7 */        
/*            I                    I  to selectable list for PAS      */        
/* 06/24/2010 I  Deepa Raju        I Task 9681-Add new Condition Code */        
/*            I                    I  DR to selectable list for PAS   */        
/*************I********************I***********************************/   
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETSELECTABLECONDITIONCODES (  )
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL
	SPECIFIC GETSLCCS
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
    ( SELECT 0 AS OID , 
	TRIM ( QTKEY ) AS CODE , 
	TRIM ( QTCCDS ) AS DESCRIPTION 
	FROM HPADQTCC 
	WHERE QTHSP# = 999 
	AND TRIM(QTKEY) in 
	('A5','A6','B4','DR','G0','P7','02','03','06','07','26','38','39','40','41','42','59','67','68') 
	
	UNION ALL 
	SELECT 0 AS OID , '' AS CODE , '' AS DESCRIPTION FROM SYSIBM / SYSDUMMY1 ) 
		AS V ( OID , CODE , DESCRIPTION )
	order by CODE; 

OPEN CURSOR1 ; 

END P1  ;

