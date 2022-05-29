                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  RELEASERESERVEDBED - SQL PROC FOR PX                 */        
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
                                                                                
CREATE PROCEDURE RELEASERESERVEDBED (                                           
        IN P_OLD_NURSINGSTATION VARCHAR(2) ,                                    
        IN P_OLD_ROOM VARCHAR(4) ,                                              
        IN P_OLD_BED VARCHAR(2) ,                                               
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC RELRESBED                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        IF P_OLD_NURSINGSTATION IS NOT NULL                                     
        AND P_OLD_ROOM IS NOT NULL                                              
        AND P_OLD_BED IS NOT NULL                                               
        THEN                                                                    
                                                                                
        UPDATE HPADLRP                                                          
        SET LRFLG = ''                                                          
        WHERE                                                                   
        LRHSP# = P_FACILITYID                                                   
        AND LRNS = P_OLD_NURSINGSTATION                                         
        AND LRROOM = P_OLD_ROOM                                                 
        AND LRBED = P_OLD_BED ;                                                 
        END IF ;                                                                
        END P1  ;                                                               
