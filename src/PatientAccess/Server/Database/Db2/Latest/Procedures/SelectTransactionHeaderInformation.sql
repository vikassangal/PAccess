                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTTRANSACTIONHEADERINFORMATION - SQL PROC FOR PX */        
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
/* 09/29/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
                                                                                
CREATE PROCEDURE SELECTTRANSACTIONHEADERINFORMATION (                           
        IN P_INSURANCETXN INTEGER ,                                             
        IN P_NONINSURANCETXN INTEGER ,                                           
        IN P_OTHERTXN INTEGER ,                                                    
        IN P_HSP INTEGER )                                                      
                                                                                
        DYNAMIC RESULT SETS 1                                                    
        LANGUAGE SQL                                                             
        SPECIFIC SELTRHDR                                                       
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                     
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN ATOMIC                                                       
        DECLARE RR# INTEGER ;                                                   
        DECLARE INSRR# INTEGER ;                                                
        DECLARE LOGNUMBER INTEGER ;                                             
        DECLARE OTHERRECNO INTEGER ;   
                                                 
        --DECLARE CURSOR1 CURSOR FOR                                              
        --SELECT                                                                  
        --QCPAUX AS DAYS_SINCE_ADMISSION                                          
        --FROM HPADQCP3                                                           
        --WHERE QCHSP# = P_HSP ;                                                  
                                                                                
        DECLARE CURSOR3 CURSOR FOR                                              
        SELECT                                                                  
        APNXA1 + P_NONINSURANCETXN ,    -- APNXA1 = Next available key for Log File 1 ( Patient RR # {seed} )                                        
        APINLH + 1 ,                    -- APINLH = Input log number ( the log number for this group of txns )                                        
        APNXA2 + P_INSURANCETXN ,       -- APNXA2 = Next available key for Log File 2 ( Insurance RR # seed )                                        
        APNXA4 + P_OTHERTXN             -- APNXA4 = Next available key for Log File 4 ( Other RR # seed )                                        
        FROM HPADAPHD                                                           
        FOR UPDATE ;                                                            
                                                                                
        DECLARE CURSOR2 CURSOR FOR                                              
        SELECT RR# AS PATIENTRELATIVERECORDNUMBER ,                             
        INSRR# AS INSURANCERELATIVERECORDNUMBER ,                               
        LOGNUMBER AS INPUTLOGNUMBER ,                                           
        OTHERRECNO AS OTHERRECORDNUMBER                                         
        FROM SYSIBM/SYSDUMMY1 ;     
                                                    
        OPEN CURSOR3 ;                                                          
        FETCH CURSOR3 INTO RR# ,                                                
        LOGNUMBER ,                                                             
        INSRR# ,                                                                
        OTHERRECNO ;                                                            
                                                                                
        UPDATE HPADAPHD SET APNXA1 = APNXA1 + P_NONINSURANCETXN ,      -- bump the patient rr # seed by the number of non-ins txns         
        APINLH = APINLH + 1 ,                                          -- bump the log # by 1         
        APNXA2 = APNXA2 + P_INSURANCETXN ,                             -- bump the insurance rr # seed by the number of ins txns         
        APNXA4 = APNXA4 + P_OTHERTXN                                   -- bump the other rr # seed by the number of other txns        
        WHERE CURRENT OF CURSOR3 ;                                              
        CLOSE CURSOR3 ;                                                         
        --OPEN CURSOR1 ;                                                          
        OPEN CURSOR2 ;                                                          
        END P1  ;                                                               
