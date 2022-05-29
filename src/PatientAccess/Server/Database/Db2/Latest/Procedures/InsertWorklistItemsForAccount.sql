                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  INSERTWORKLISTITEMSFORACCOUNT - SQL PROC FOR PX      */        
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
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE INSERTWORKLISTITEMSFORACCOUNT (                                
        IN P_FACILITYID INTEGER ,                                               
        IN P_ACCOUNTNUMBER INTEGER ,                                            
        IN P_WORKLISTID INTEGER ,                                               
        IN P_COMPOSITERULEID INTEGER ,                                          
        IN P_RULEID INTEGER )                                                   
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC INSWRKLSTA                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        INSERT INTO ACCOUNTWORKLISTITEMS                                        
        ( FACILITYID , ACCOUNTNUMBER ,                                          
        WORKLISTID , COMPOSITERULEID , RULEID )                                 
        VALUES (P_FACILITYID , P_ACCOUNTNUMBER ,                                
                P_WORKLISTID , P_COMPOSITERULEID , P_RULEID) ;                  
        END P1  ;                                                               
