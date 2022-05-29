                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  MRNFORACCOUNT - SQL PROC FOR PX                      */        
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
CREATE PROCEDURE MRNFORACCOUNT (                                                
        IN P_ACCOUNTNUMBER INTEGER ,                                            
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC MRNFORACC                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
                                                                                
        SELECT                                                                  
                                                                                
        RWMRC# AS MRN                                                           
                                                                                
        FROM HXMRRWP                                                            
                                                                                
        WHERE                                                                   
                                                                                
        RWHSP# = P_FACILITYID AND                                               
                                                                                
        RWACCT = P_ACCOUNTNUMBER;                                               
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
