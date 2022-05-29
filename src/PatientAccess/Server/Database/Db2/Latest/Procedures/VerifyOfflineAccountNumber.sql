                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  VERIFYOFFLINEACCOUNTNUMBER - SQL PROC FOR PX         */        
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
/* 04/28/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
CREATE PROCEDURE VERIFYOFFLINEACCOUNTNUMBER (                                   
                                                                                
        IN P_ACCOUNTNUMBER INTEGER ,                                            
                                                                                
        IN P_ACCOUNTNUMBERSEED INTEGER ,                                        
                                                                                
        IN P_HSP INTEGER ,                                                      
                                                                                
        OUT O_RESULT VARCHAR(100) )                                             
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC VEROFFACC                                                      
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW = *SOURCE                                            
        BEGIN                                                                   
                                                                                
                                                                                
                                                                                
                DECLARE REC_COUNT INTEGER DEFAULT 0 ;                           
                DECLARE NEXT_ACCTNUMSEED INTEGER DEFAULT 0 ;                    
                                                                                
                                                                                
                                                                                
                SELECT COUNT ( RWACCT )                                         
                INTO REC_COUNT FROM HXMRRWP                                     
                                                                                
                WHERE RWHSP# = P_HSP AND                                        
                                                                                
                RWACCT = P_ACCOUNTNUMBER ;                                      
                                                                                
                                                                                
                                                                                
                IF REC_COUNT > 0 THEN                                           
                                                                                
                SET O_RESULT = 'InUse' ;                                        
                                                                                
                ELSE                                                            
                                                                                
                SELECT                                                          
                QCNAC# INTO NEXT_ACCTNUMSEED                                    
                FROM HPADQCP2                                                   
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                                                                                
                                                                                
                IF P_ACCOUNTNUMBERSEED < NEXT_ACCTNUMSEED THEN                  
                                                                                
                SET O_RESULT = 'NotInUse' ;                                     
                                                                                
                END IF ;                                                        
                                                                                
                END IF ;                                                        
                                                                                
                                                                                
                                                                                
                END  ;                                                          
