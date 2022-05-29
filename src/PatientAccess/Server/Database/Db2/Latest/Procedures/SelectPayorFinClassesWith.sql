                                                                                   
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTPAYORFINCLASSESWITH - SQL PROC FOR PX          */        
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
/* 01/24/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPAYORFINCLASSESWITH( 
		IN @P_PAYORCODE VARCHAR(3),                                                  
        IN @P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPYRFINW                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN
				DECLARE CURSOR1 CURSOR FOR                  
                SELECT 
                 TRIM(PAYORFINXREF.PCFCCD) AS FINANCIALCLASSCODE
                FROM NHADPCP PAYORFINXREF
				WHERE TRIM(PAYORFINXREF.PCPYCD) = @P_PAYORCODE
                   AND PAYORFINXREF.PCHSP# = @P_FACILITYID;                                                                            
                OPEN CURSOR1 ; 
                                                                 
                END P1  ;                                                       
