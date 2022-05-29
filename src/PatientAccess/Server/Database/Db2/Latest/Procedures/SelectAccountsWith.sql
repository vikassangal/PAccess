                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTSWITH - SQL PROC FOR PX                 */        
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
                                                                                
CREATE PROCEDURE SELECTACCOUNTSWITH (                                           
        IN P_FACILITYID INTEGER ,                                               
        IN P_STARTLETTERS VARCHAR(3) ,                                          
        IN P_ENDLETTERS VARCHAR(3) ,                                            
        IN P_STARTDATE DATE ,                                                   
        IN P_ENDDATE DATE ,                                                     
        IN P_WORKLISTID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELACCWITH                                                     
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
                HSV.SERVICEID AS SERVICEID ,                                    
                HSV.SERVICECODE AS SERVICECODE ,                                
                HSV.SERVICEDESCRIPTION AS SERVICEDESCRIPTION ,                  
                HC.CLINICCODEID AS CLINICCODEID ,                               
                HC.CLINICCODE AS CLINICCODE ,                                   
                HC.DESCRIPTION AS CLINICDESCRIPTION ,                           
                FC.FINANCIALCODE AS FINANCIALCODE ,                             
                PAY.PAYORNAME AS PAYORNAME ,                                    
                N.FIRSTNAME AS FIRSTNAME ,                                      
                N.LASTNAME AS LASTNAME ,                                        
                A.LPPTYP AS PATIENTTYPECODE ,                                   
                PT.PATIENTTYPEDESCRIPTION AS PATIENTTYPEDESC ,                  
                A.LPPSTO ,                                                      
                A.LPPSTI ,                                                      
                A.LPFBIL ,                                                      
                A.LPVIS# AS LPVIS ,                                             
                A.LPPOLN ,                                                      
                A.LPUBAL ,                                                      
                A.LPMSV ,                                                       
                L.LFRCID AS LOCKINDICATOR ,                                     
                ( SELECT COUNT ( * )                                            
                FROM REMAININGACTIONS RA ,                                      
                WORKLISTITEMS WLI                                               
                WHERE WLI.VISITID = V.VISITID                                   
                AND WLI.WORKLISTID = P_WORKLISTID                               
                AND RA.WORKLISTITEMID = WLI.WORKLISTITEMID )                    
                                AS ACTIONCOUNT                                  
                FROM VISITS V                                                   
                JOIN PATIENTS P ON ( P.PATIENTID = V.PATIENTID )                
                JOIN NAMES N ON ( N.PERSONID = P.PERSONID )                     
                LEFT OUTER JOIN SERVICES HSV ON                                 
                ( V.SERVICEID = HSV.SERVICEID )                                 
                LEFT OUTER JOIN HOSPITALCLINICS HC ON                           
                ( HC.CLINICCODEID = V.CLINICCODEID )                            
                LEFT OUTER JOIN FINANCIALCLASSES FC ON                          
                ( FC.FINANCIALCLASSID = V.FINANCIALCLASSID )                    
                LEFT OUTER JOIN PAYORS PAY ON                                   
                ( PAY.PAYORID = V.PAYORID )                                     
                LEFT OUTER JOIN NQHRLFP L ON                                    
                ( V.ACCOUNTNUMBER = L.LFKEY                                     
                AND L.LFHSP# = P_FACILITYID )                                   
                JOIN HPADLPP A ON                                               
                ( LPACCT = V.ACCOUNTNUMBER                                      
                AND LPHSP# = P_FACILITYID )                                     
                JOIN PATIENTTYPES PT ON                                         
                ( PT.PATIENTTYPECODE = A.LPPTYP )                               
                WHERE P.FACILITYID = P_FACILITYID                               
                AND ( P_STARTLETTERS IS NULL                                    
                OR UPPER ( N.LASTNAME ) >= UPPER ( P_STARTLETTERS ) )           
                AND ( P_ENDLETTERS IS NULL                                      
                OR UPPER ( N.LASTNAME ) <= UPPER ( P_ENDLETTERS ) )             
                AND (                                                           
                ( P_STARTDATE IS NULL ) OR                                      
                ( P_STARTDATE IS NOT NULL                                       
                AND P_ENDDATE IS NOT NULL                                       
                AND V.ADMISSIONDATE >= P_STARTDATE                              
                AND V.ADMISSIONDATE <= P_ENDDATE ) )                            
                AND ( SELECT COUNT ( * )                                        
                FROM REMAININGACTIONS RA ,                                      
                WORKLISTITEMS WLI                                               
                WHERE WLI.VISITID = V . VISITID                                 
                AND WLI.WORKLISTID = P_WORKLISTID                               
                AND RA.WORKLISTITEMID = WLI.WORKLISTITEMID ) > 0 ;              
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
