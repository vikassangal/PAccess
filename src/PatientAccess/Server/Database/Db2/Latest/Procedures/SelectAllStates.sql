                                                                                 
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLSTATES - SQL PROC FOR PX        */        
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
/* 01/15/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/ 
/* 09/24/2012 I  Deepak Nair       I  Added a new field - STATENUMBER */        
/*************I********************I***********************************/       
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTALLSTATES()                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELALLST                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT   
                                                         
        SET OPTION DBGVIEW = *SOURCE
                                                    
        P1 : BEGIN              
                                                        
			DECLARE CURSOR1 CURSOR FOR                                      
        
        
			SELECT 		DISTINCT
			0			AS OID,
			TRIM(QTKEY) AS CODE,
			TRIM(QTSTN) AS DESCRIPTION,
			TRIM ( QTSTNUM ) AS STATENUMBER
			
			FROM HPADQTSZ
			
			WHERE QTHSP# = 999
			
			UNION
			       
			SELECT		0 AS OID , 
						'' AS CODE , 
						'' AS DESCRIPTION,
						'' AS STATENUMBER
			FROM	SYSIBM / SYSDUMMY1 
			 			
			ORDER BY DESCRIPTION ; 
	                                
			OPEN CURSOR1 ; 
			                                                 
        END P1  ;                                                       
