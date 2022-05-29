                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTSWITHWORKLISTS - SQL PROC FOR PX        */        
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
                                                                                
CREATE PROCEDURE SELECTACCOUNTSWITHWORKLISTS (                                  
        IN P_FACILITYID INTEGER ,                                               
        IN P_STARTINGLETTERS VARCHAR(3) ,                                       
        IN P_ENDINGLETTERS VARCHAR(3) ,                                         
        IN P_STARTDATE DATE ,                                                   
        IN P_ENDDATE DATE ,                                                     
        IN P_WORKLISTID INTEGER ,                                               
        IN P_RANGEID INTEGER )                                                  
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELACCWKL                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE STARTDATE DATE DEFAULT NULL ;                           
                DECLARE ENDDATE DATE DEFAULT NULL ;                             
                DECLARE CURRDATE DATE DEFAULT NULL ;                            
                DECLARE PERIODSPAN INTEGER DEFAULT 0 ;                          
                DECLARE CURSOR1 CURSOR FOR                                      
                        SELECT                                                  
                        V.ACCOUNTNUMBER AS ACCOUNTNUMBER ,                      
                        V.MEDICALRECORDNUMBER AS MEDICALRECORDNUMBER ,          
                        V.ADMISSIONDATE AS ADMISSIONDATE ,                      
                        V.DISCHARGEDATE AS DISCHARGEDATE ,                      
                        V.SERVICECODE AS SERVICECODE ,                          
                        V.CLINICCODEID AS CLINICCODEID ,                        
                        V.CLINICCODE AS CLINICCODE ,                            
                        V.CLINICDESCRIPTION AS CLINICDESCRIPTION ,              
                        V.FINANCIALCODE AS FINANCIALCODE ,                      
                        E.HBINM AS PAYORNAME ,                                  
                        V.FIRSTNAME AS FIRSTNAME ,                              
                        V.LASTNAME AS LASTNAME ,                                
                        V.PATIENTTYPE AS PATIENTTYPE ,                          
                        --v.LPPSTO,                                             
                        --v.LPPSTI,                                             
                        --v.LPFBIL,                                             
                        --v.LPVIS,                                              
                        --v.LPPOLN,                                             
                        --v.LPUBAL,                                             
                        --v.LPMSV,                                              
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
                        V.LOCKTIME ,                                            
                        V.ACCOMODATIONCODE AS ACCOMODATIONCODE ,                
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
                        WHERE                                                   
                        V.FACILITYID = P_FACILITYID                             
                        AND ( ( ( P_WORKLISTID != 6 )                           
                        OR ( P_WORKLISTID = 6                                   
                        AND TRIM ( V.IPSTATUSCODE ) != 'G' ) ) )                
                        AND ( P_STARTINGLETTERS IS NULL OR                      
                        SUBSTR ( UPPER ( V.LASTNAME ) , 1 ,                     
                        LENGTH ( TRIM ( P_STARTINGLETTERS ) ) ) >=              
                        UPPER ( TRIM ( P_STARTINGLETTERS ) ) )                  
                        AND ( P_ENDINGLETTERS IS NULL OR                        
                        SUBSTR ( UPPER ( V.LASTNAME ) , 1 ,                     
                        LENGTH ( TRIM ( P_ENDINGLETTERS ) ) ) <=                
                        UPPER ( TRIM ( P_ENDINGLETTERS ) ) )                    
                        AND ( ( STARTDATE IS NULL ) OR                          
                        ( STARTDATE IS NOT NULL                                 
                        AND ENDDATE IS NOT NULL                                 
                        AND DATE ( V.ADMISSIONDATE ) >= STARTDATE               
                        AND DATE ( V.ADMISSIONDATE ) <= ENDDATE                 
                        )                                                       
                        )                                                       
                        AND ( EXISTS                                            
                        ( SELECT RA.WORKLISTITEMID                              
                        FROM REMAININGACTIONS RA , WORKLISTITEMS WLI            
                        WHERE WLI.FACILITYID = P_FACILITYID                     
                        AND WLI.ACCOUNTNUMBER = V.ACCOUNTNUMBER                 
                        AND WLI.WORKLISTID = P_WORKLISTID                       
                        AND RA.WORKLISTITEMID = WLI.WORKLISTITEMID ) )          
                         -- if the account is on the NOshow worklist (6)        
                         -- or ED/PreMSE worklist (5) then only display         
                         -- it if we are asking for no show.                    
                         -- If it is ALSO on another list                       
                         -- it should only appear on the no list or Pre-MSE list
                        AND ( P_WORKLISTID = 6 OR                               
                        P_WORKLISTID = 5                                        
                        OR                                                      
                        ( NOT EXISTS                                            
                        ( SELECT WORKLISTITEMID                                 
                        FROM WORKLISTITEMS W2                                   
                        WHERE ( ( W2.WORKLISTID = 6 OR  -- no Show              
                        W2.WORKLISTID = 5 )  -- PreMSE                          
                        AND W2.FACILITYID = P_FACILITYID                        
                        AND W2.ACCOUNTNUMBER = V.ACCOUNTNUMBER ) ) ) ) ;        
              SELECT CURRENT DATE INTO CURRDATE FROM SYSIBM/SYSDUMMY1 ;         
                         --Select rangeindays as  periodSpan                    
                        SELECT RANGEINDAYS INTO PERIODSPAN                      
                        FROM WORKLISTSELECTIONRANGES                            
                        WHERE WORKLISTSELECTIONRANGEID = P_RANGEID ;            
                        IF P_RANGEID <> 9 AND P_RANGEID <> 8 THEN               
                        IF PERIODSPAN < 0 THEN                                  
                        SET STARTDATE = CURRDATE + PERIODSPAN DAYS ;            
                        SET ENDDATE = CURRDATE ;                                
                        ELSE                                                    
                        SET STARTDATE = CURRDATE ;                              
                        SET ENDDATE = CURRDATE + PERIODSPAN DAYS ;              
                        END IF ;                                                
                        ELSE                                                    
                        SET STARTDATE = P_STARTDATE ;                           
                        SET ENDDATE = P_ENDDATE ;                               
                        END IF ;                                                
                        OPEN CURSOR1 ;                                          
                        END P1  ;                                               
