                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTMRNSPAN - SQL PROC FOR PX                      */        
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
                                                                                
CREATE PROCEDURE SELECTMRNSPAN (                                                
        IN P_HSP INTEGER ,                                                      
        IN P_SPANSIZE INTEGER )                                                 
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELMRNSP                                                       
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN ATOMIC                                                       
                        DECLARE NEXTNUMBER INTEGER ;                            
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        NEXTNUMBER AS NEXTMRN                                   
                        FROM SYSIBM/SYSDUMMY1 ;                                 
                                                                                
                        SELECT                                                  
                        QCNMR# INTO NEXTNUMBER                                  
                        FROM HPADQCP2                                           
                        WHERE QCHSP# = P_HSP ;                                  
                                                                                
                        UPDATE HPADQCP2                                         
                        SET QCNMR# = QCNMR# + P_SPANSIZE                        
                        WHERE QCHSP# = P_HSP ;                                  
                                                                                
                        OPEN CURSOR1 ;                                          
                END P1  ;                                                       
