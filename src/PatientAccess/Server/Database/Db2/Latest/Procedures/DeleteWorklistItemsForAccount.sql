                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  DELETEWORKLISTITEMSFORACCOUNT - SQL PROC FOR PX      */        
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
                                                                                
CREATE PROCEDURE DELETEWORKLISTITEMSFORACCOUNT (                                
        IN P_FACILITYID INTEGER ,                                               
        IN P_ACCOUNTNUMBER INTEGER )                                            
        LANGUAGE SQL                                                            
        SPECIFIC DLTWRKACC                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DELETE FROM                                                             
        ACCOUNTWORKLISTITEMS WLI                                                
                                                                                
        WHERE WLI . ACCOUNTNUMBER = P_ACCOUNTNUMBER                             
        AND WLI . FACILITYID = P_FACILITYID ;                                   
        END P1  ;                                                               
