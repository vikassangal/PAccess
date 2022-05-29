                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTSFOR - SQL PROC FOR PX                  */        
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
                                                                                
CREATE PROCEDURE SELECTACCOUNTSFOR (                                            
        IN P_HSP INTEGER ,                                                      
        IN P_MRC INTEGER ,                                                      
        IN P_PATIENTTYPE_PREREG VARCHAR(1) ,                                    
        IN P_PATIENTTYPE_INPAT VARCHAR(1) ,                                     
        IN P_PATIENTTYPE_OUTPATIENT VARCHAR(1) ,                                
        IN P_PATIENTTYPE_PREMSE VARCHAR(1) ,                                    
        IN P_PATIENTTYPE_POSTMSE VARCHAR(1) ,                                   
        IN P_PATIENTTYPE_RECUR VARCHAR(1) ,                                     
        IN P_PATIENTTYPE_NONPAT VARCHAR(1) )                                    
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELACCFOR                                                      
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
                V.ACCOMODATIONCODE ,                                            
                V.ACCOMODATIONDESC ,
                V.VALUABLESARETAKEN AS VALUABLESARETAKEN ,                      
                V.PENDINGDISCHARGE AS PENDINGDISCHARGE ,                        
                V.ABSTRACTEXISTS AS ABSTRACTEXISTS ,                            
                V.DISCHARGECODE AS DISCHARGECODE ,                              
                V.OPERATINGDRID ,                                               
                V.ATTENDINGDRID ,                                               
                V.ADMITTINGDRID ,                                               
                V.REFERINGDRID ,                                                
                V.OTHERDRID ,                                                   
                V.CONSULTINGDR1ID ,                                             
                V.CONSULTINGDR2ID ,                                             
                V.CONSULTINGDR3ID ,                                             
                V.CONSULTINGDR4ID ,                                             
                V.CONSULTINGDR5ID ,                                             
                V.ADMITSOURCECODE ,                                             
                E.HBINS# AS PRIMARYPLANID ,                                     
                V.LOCKINDICATOR ,                                               
                V.LOCKERPBARID ,                                                
                V.LOCKDATE ,                                                    
                V.LOCKTIME ,                                                    
                V.LASTMAINTENANCEDATE AS LASTMAINTENANCEDATE ,                  
                V.LASTMAINTENANCELOGNUMBER                                      
                        AS LASTMAINTENANCELOGNUMBER ,                           
                V.UPDATELOGNUMBER AS UPDATELOGNUMBER                            
                FROM ACCOUNTPROXIES V                                           
                LEFT JOIN HPADHBP E ON                                          
                ( V.FACILITYID = E.HBHSP#                                       
                AND V.MEDICALRECORDNUMBER = E.HBMRC#                            
                AND V.ACCOUNTNUMBER = E.HBACCT                                  
                AND E.HBSEQ# = 1 )                                              
                WHERE V.FACILITYID = P_HSP                                      
                AND V.MEDICALRECORDNUMBER = P_MRC                               
                AND (                                                           
                ( P_PATIENTTYPE_PREREG IS NOT NULL                              
                AND V.PATIENTTYPE = '0' )                                       
                OR                                                              
                ( P_PATIENTTYPE_INPAT IS NOT NULL                               
                AND V.PATIENTTYPE = '1' )                                       
                OR                                                              
                ( P_PATIENTTYPE_OUTPATIENT IS NOT NULL                          
                AND V.PATIENTTYPE = '2' )                                       
                OR                                                              
                ( P_PATIENTTYPE_PREMSE IS NOT NULL                              
                AND V.PATIENTTYPE = '3'                                         
                AND V.FINANCIALCODE = '37' )                                    
                OR                                                              
                ( P_PATIENTTYPE_POSTMSE IS NOT NULL                             
                AND V.PATIENTTYPE = '3' )                                       
                OR                                                              
                ( P_PATIENTTYPE_RECUR IS NOT NULL                               
                AND V.PATIENTTYPE = '4' )                                       
                OR                                                              
                ( P_PATIENTTYPE_NONPAT IS NOT NULL                              
                AND V.PATIENTTYPE = '9' )                                       
                )                                                               
                ORDER BY DISCHARGEDATE DESC ,                                   
                ADMISSIONDATE DESC ,                                            
                ACCOUNTNUMBER DESC ;                                            
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
