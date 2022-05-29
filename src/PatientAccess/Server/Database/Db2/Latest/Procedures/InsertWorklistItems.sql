                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  INSERTWORKLISTITEMS - SQL PROC FOR PX                */        
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
                                                                                
CREATE PROCEDURE INSERTWORKLISTITEMS (                                          
        IN P_FACILITYID INTEGER ,                                               
        IN P_WORKLISTID INTEGER ,                                               
        IN P_ACCOUNTNUMBER INTEGER )                                            
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC INSWRKLST                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        INSERT INTO WORKLISTITEMS VALUES (                                      
        NEXTVAL FOR WORKILISTITEMSEQ ,                                          
        P_WORKLISTID ,                                                          
        P_FACILITYID ,                                                          
        P_ACCOUNTNUMBER ) ;                                                     
                                                                                
        END P1  ;                                                               
