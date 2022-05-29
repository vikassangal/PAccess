                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCLCKSTS  - STORED PROCEDURE FOR PX                 */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
/* 07/06/2006 I                    I Modified STORED PROCEDURE        */        
/* 07/18/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
                                                                                
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
CREATE PROCEDURE ACCOUNTLOCKSTATUSFOR (                                         
                                                                                
        IN P_HSP INTEGER ,                                                      
                                                                                
        IN P_ACCOUNTNUMBER CHAR(9) )                                            
                                                                                
        DYNAMIC RESULT SETS 1                                                   
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC ACCLCKSTS                                                      
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                                                                                
                SELECT                                                          
                                                                                
                LFRCID AS LOCKINDICATOR ,                                       
                                                                                
                LFUSER AS LOCKERPBARID ,                                        
                                                                                
                LFTDAT AS LOCKDATE ,                                            
                                                                                
                LFJOBN AS LOCKEDWORKSTATION ,                                   
                                                                                
                LFTTIM AS LOCKTIME                                              
                                                                                
                FROM NQHRLFP                                                    
                                                                                
                WHERE LFKEY = P_ACCOUNTNUMBER                                   
                                                                                
                AND LFHSP# = P_HSP ;                                            
                                                                                
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
