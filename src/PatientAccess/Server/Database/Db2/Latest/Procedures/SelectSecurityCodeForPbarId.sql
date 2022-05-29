                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTSECURITYCODEFORPBARID - SQL PROC FOR PX        */        
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
                                                                                
CREATE PROCEDURE SELECTSECURITYCODEFORPBARID (                                  
        IN P_PBARID VARCHAR(8) )                                                
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELSECPBAR                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN ATOMIC                                                       
                                DECLARE CURSOR1 CURSOR FOR                      
                                SELECT                                          
                                EISEC2 AS SECURITYCODE                          
                                FROM EI0008P                                    
                                WHERE EIEMP# = P_PBARID ;                       
                                OPEN CURSOR1 ;                                  
                        END P1  ;                                               
