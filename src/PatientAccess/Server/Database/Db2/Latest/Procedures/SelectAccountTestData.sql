                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTTESTDATA - SQL PROC FOR PX              */        
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
                                                                                
CREATE PROCEDURE SELECTACCOUNTTESTDATA (                                        
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELACCTDTA                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT                                                          
                AV.FACILITYID ,                                                 
                AV.ACCOUNTNUMBER ,                                              
                AV.ADMISSIONDATE ,                                              
                AV.DISCHARGEDATE ,                                              
                AV.SERVICECODE ,                                                
                AV.CLINICCODEID ,                                               
                AV.CLINICCODE ,                                                 
                AV.CLINICDESCRIPTION ,                                          
                AV.FINANCIALCODE ,                                              
                AV.FIRSTNAME ,                                                  
                AV.LASTNAME ,                                                   
                AV.MIDDLEINITIAL ,                                              
                AV.PATIENTTYPE ,                                                
                AV.OPSTATUSCODE ,                                               
                AV.IPSTATUSCODE ,                                               
                AV.FINALBILLINGFLAG ,                                           
                AV.OPVISITNUMBER ,                                              
                AV.PENDINGPURGE ,                                               
                AV.UNBILLEDBALANCE ,                                            
                AV.MEDICALSERVICECODE ,                                         
                AV.LOCKINDICATOR ,                                              
                AV.LOCKERPBARID ,                                               
                AV.LOCKDATE ,                                                   
                AV.LOCKTIME ,                                                   
                AV.NURSINGSTATION ,                                             
                AV.ROOM ,                                                       
                AV.BED ,                                                        
                AV.ISOLATIONCODE ,                                              
                AV.PENDINGDISCHARGE ,                                           
                AV.DISCHARGECODE ,                                              
                AV.VALUABLESARETAKEN ,                                          
                AV.ABSTRACTEXISTS ,                                             
                AV.CHIEFCOMPLAINT ,                                             
                AV.MEDICALRECORDNUMBER ,                                        
                AV.DOB ,                                                        
                AV.GENDERID ,                                                   
                AV.OPTOUT AS OPTOUT ,                                           
                AV.CONFIDENTIAL                                                 
                FROM ACCOUNTPROXIES AV                                          
                WHERE AV.FACILITYID = P_FACILITYID                              
                FETCH FIRST 2000 ROWS ONLY ;                                    
                ---/* select only columns which are required,                   
                -- also look for a option if we can randomly                    
                -- pick 1000 - 2000 records */                                  
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
