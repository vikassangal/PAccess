                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  INSERTREMAININGACTIONS - SQL PROC FOR PX             */        
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
/* 04/27/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE INSERTREMAININGACTIONS (                                       
        IN P_FACILITYID INTEGER ,                                               
        IN P_WORKLISTID INTEGER ,                                               
        IN P_ACTIONID INTEGER ,                                                 
        IN P_ACCOUNTNUMBER INTEGER )                                            
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC INSREMACT                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        INSERT INTO REMAININGACTIONS VALUES (                                   
        P_ACTIONID ,                                                            
        WORKLISTITEMIDFOR ( P_ACCOUNTNUMBER ,                                   
        P_FACILITYID ,                                                          
        P_WORKLISTID )                                                          
        ) ;                                                                     
                                                                                
        END P1  ;                                                               
