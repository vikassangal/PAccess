                                                                                   
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTCOUNTRYWITH - SQL PROC FOR PX        */        
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
/* 01/23/2008 I  GAUTHREAUX        I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/
/* 09/24/2012 I  Deepak Nair       I  Added a new field - STATECODE   */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTCOUNTYWITH(
		IN @P_FACILITYID INT,
		IN @P_CODE VARCHAR(10) )  
		                                             
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELCNTYWTH                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                                  
        CALLED ON NULL INPUT                                                    
        
        SET OPTION DBGVIEW = *SOURCE                                            
        
        P1 : BEGIN                                                              
        
                DECLARE CURSOR1 CURSOR FOR                                      
        
                SELECT 	DISTINCT
                0 AS OID,
				TRIM(QTKEY) AS CODE,
				TRIM(QTCNTN) AS DESCRIPTION,
				TRIM (QTSTATE) AS STATECODE 
				FROM	HPADQTCO
				WHERE	QTHSP# = @P_FACILITYID
				AND		QTKEY = @P_CODE
				;                                        
        
                OPEN CURSOR1 ;                                                  
        
        END P1  ;                                                       
