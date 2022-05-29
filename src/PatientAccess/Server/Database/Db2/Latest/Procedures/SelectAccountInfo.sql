                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTINFO - SQL PROC FOR PX                  */        
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
                                                                                
CREATE PROCEDURE SELECTACCOUNTINFO (                                            
        IN P_HSP INTEGER ,                                                      
        IN P_MRC INTEGER )                                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELACCINFO                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        V.ACCOUNTNUMBER AS ACCOUNTNUMBER ,                      
                        V.ADMISSIONDATE AS ADMISSIONDATE ,                      
                        V.DISCHARGEDATE AS DISCHARGEDATE ,                      
                        V.SERVICECODE AS SERVICECODE ,                          
                        V.CLINICCODEID AS CLINICCODEID ,                        
                        V.CLINICCODE AS CLINICCODE ,                            
                        V.CLINICDESCRIPTION AS CLINICDESCRIPTION ,              
                        V.FINANCIALCODE AS FINANCIALCODE ,                      
                        V.PATIENTTYPE AS PATIENTTYPE ,                          
                        V.OPSTATUSCODE AS OPSTATUSCODE ,                        
                        V.IPSTATUSCODE AS IPSTATUSCODE ,                        
                        V.FINALBILLINGFLAG AS FINALBILLINGFLAG ,                
                        V.OPVISITNUMBER AS OPVISITNUMBER ,                      
                        V.PENDINGPURGE AS PENDINGPURGE ,                        
                        V.UNBILLEDBALANCE AS UNBILLEDBALANCE ,                  
                        V.MEDICALSERVICECODE AS MEDICALSERVICECODE ,            
                        V.NURSINGSTATION AS NURSINGSTATION ,                    
                        V.ROOM AS ROOM ,                                        
                        V.BED AS BED ,                                          
                        V.VALUABLESARETAKEN AS VALUABLESARETAKEN ,              
                        V.PENDINGDISCHARGE AS PENDINGDISCHARGE ,                
                        V.ABSTRACTEXISTS AS ABSTRACTEXISTS ,                    
                        V.DISCHARGECODE AS DISCHARGECODE ,                      
                        V.LOCKINDICATOR ,                                       
                        V.LOCKERPBARID ,                                        
                        V.LOCKDATE ,                                            
                        V.LOCKTIME                                              
                        FROM ACCOUNTPROXIES V                                   
                        LEFT OUTER JOIN NQHRLFP L ON                            
                        ( V.ACCOUNTNUMBER = L.LFKEY                             
                        AND L.LFHSP# = P_HSP )                                  
                        WHERE V.FACILITYID = P_HSP                              
                        AND V.MEDICALRECORDNUMBER = P_MRC                       
                        ORDER BY ADMISSIONDATE DESC ,                           
                        ACCOUNTNUMBER DESC ;                                    
                        OPEN CURSOR1 ;                                          
                        END P1  ;                                               
