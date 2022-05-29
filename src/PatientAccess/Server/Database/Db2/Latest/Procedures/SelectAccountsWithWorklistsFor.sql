                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTSWITHWORKLISTSFOR - SQL PROC FOR PX     */        
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
/* 09/07/2006 I                    I MODIFIED STORED PROCEDURE        */        
/* 09/37/2007 I   Gauthreaux       I Flip-Flopped tables              */
/* 02/28/2008 I  KJS               I remove CHGQRYA call              */
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
                                                                                
CREATE PROCEDURE SELECTACCOUNTSWITHWORKLISTSFOR (                                                                                                  
        IN P_FACILITYID INTEGER ,                                                                                                                     
        IN P_STARTINGLETTERS VARCHAR(3) ,                                                                                                          
        IN P_ENDINGLETTERS VARCHAR(3) ,                                                                                                              
        IN P_STARTDATE DATE ,                                                                                                                        
        IN P_ENDDATE DATE ,                                                                                                                          
        IN P_WORKLISTID INTEGER ,                                                                                                                      
        IN P_RANGEID INTEGER )                                                  
                                                                                
        DYNAMIC RESULT SETS 1                                                                                                                        
        LANGUAGE SQL                                                                                                                        
        SPECIFIC SELACTWKLF                                                      
        NOT DETERMINISTIC                                                        
        READS SQL DATA                                                            
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                                
                                                                                 
        DECLARE STARTDATE DATE DEFAULT NULL ;                                    
        DECLARE ENDDATE DATE DEFAULT NULL ;                                      
        DECLARE CURRDATE DATE DEFAULT NULL ;                                     
        DECLARE PERIODSPAN INTEGER DEFAULT 0 ;                                  
        DECLARE NULL_COLUMN VARCHAR ( 1 ) DEFAULT NULL ;                        
                                                                                   
        DECLARE CURSOR1 CURSOR FOR                                              
                                                                                
        SELECT DISTINCT                                                            
        FIN . PMDTBL ,                                                           
        V . ACCOUNTNUMBER AS ACCOUNTNUMBER ,                                    
        V . MEDICALRECORDNUMBER AS MEDICALRECORDNUMBER ,                         
        V . ADMISSIONDATE AS ADMISSIONDATE ,                                      
        V . DISCHARGEDATE AS DISCHARGEDATE ,                                      
        V . SERVICECODE AS SERVICECODE ,                                        
        V . CLINICCODEID AS CLINICCODEID ,                                      
        V . CLINICCODE AS CLINICCODE ,                                          
        V . CLINICDESCRIPTION AS CLINICDESCRIPTION ,                            
        V . FINANCIALCODE AS FINANCIALCODE ,                                    
        E . HBINM AS PAYORNAME ,                                                
        V . FIRSTNAME AS FIRSTNAME ,                                            
        V . LASTNAME AS LASTNAME ,                                              
        V . MIDDLEINITIAL AS MIDDLEINITIAL ,                                    
        V . PATIENTTYPE AS PATIENTTYPE ,                                        
        V . OPSTATUSCODE AS OPSTATUSCODE ,                                       
        V . IPSTATUSCODE AS IPSTATUSCODE ,                                       
        V . FINALBILLINGFLAG AS FINALBILLINGFLAG ,                               
        V . OPVISITNUMBER AS OPVISITNUMBER ,                                        
        V . PENDINGPURGE AS PENDINGPURGE ,                                       
        V . UNBILLEDBALANCE AS UNBILLEDBALANCE ,                                 
        V . MEDICALSERVICECODE AS MEDICALSERVICECODE ,                          
        V . NURSINGSTATION AS NURSINGSTATION ,                                   
        V . ROOM AS ROOM ,                                                        
        V . BED AS BED ,                                                         
        V . VALUABLESARETAKEN AS VALUABLESARETAKEN ,                             
        V . PENDINGDISCHARGE AS PENDINGDISCHARGE ,                               
        V . ABSTRACTEXISTS AS ABSTRACTEXISTS ,                                   
        V . DISCHARGECODE AS DISCHARGECODE ,                                      
        V . PREDISCHARGESTATUS AS PREDISCHARGESTATUS ,                            
        V . ACCOMODATIONDESC AS ACCOMODATIONDESC ,                              
                                                                                
        CASE WHEN V . LOCKINDICATOR IS NULL THEN                                  
                'N'                                                              
        ELSE                                                                     
                'Y'                                                             
        END AS LOCKINDICATOR , V . LOCKERPBARID ,                                
        V . LOCKDATE ,                                                           
        V . LOCKTIME ,                                                          
        V . ACCOMODATIONCODE AS ACCOMODATIONCODE ,                              
        V . LASTMAINTENANCEDATE AS LASTMAINTENANCEDATE ,                        
        V . LASTMAINTENANCELOGNUMBER AS LASTMAINTENANCELOGNUMBER ,              
        V . UPDATELOGNUMBER AS UPDATELOGNUMBER ,                                
        ( SELECT COUNT ( WLI . WORKLISTITEMID )                                 
        FROM ACCOUNTWORKLISTITEMS WLI                                            
        WHERE WLI . FACILITYID = P_FACILITYID                                   
        AND WLI . ACCOUNTNUMBER = V . ACCOUNTNUMBER                              
        AND WLI . WORKLISTID = P_WORKLISTID )                                     
        AS ACTIONCOUNT ,                                                         
        HSV . QTMSVD AS HOSPITALSERVICE ,                                         
        FC . QTFCD AS FINANCIALCLASS ,                                           
        NULL_COLUMN AS FILLER                                                    
                                                                                
        FROM ACCOUNTWORKLISTITEMS W 
			JOIN  ACCOUNTPROXIES V                                                  
			ON V . FACILITYID = W . FACILITYID                                      
			AND V . ACCOUNTNUMBER = W . ACCOUNTNUMBER                               
        JOIN FF0015P FAC ON V . FACILITYID = FFHSPN                             
        JOIN PM0001P FIN ON FAC . FFHSPC = FIN . PMHSPC                         
			AND FIN . PMPT#9 = V . ACCOUNTNUMBER                                      
        LEFT JOIN HPADHBP E                                                      
			ON V . FACILITYID = E . HBHSP#                                          
			AND V . MEDICALRECORDNUMBER = E . HBMRC#                                 
			AND V . ACCOUNTNUMBER = E . HBACCT                                        
			AND E . HBSEQ# = 1                                                       
        -- HOSPITAL SERVICE CODE REFERENCE                                     
        JOIN HPADQTMS HSV                                                       
			ON HSV . QTHSP# = V . FACILITYID                                        
			AND HSV . QTKEY = V . SERVICECODE                                       
        JOIN HPADQTFC FC ON FC . QTHSP# = V . FACILITYID                        
			AND FC . QTKEY = V . FINANCIALCODE                                     
		
		WHERE V . FACILITYID = P_FACILITYID                                      
			AND W . WORKLISTID = P_WORKLISTID                                        
        AND FIN . PMDTBL = 0                                               
        AND ( P_WORKLISTID != 6                                                       
			OR                                                                      
			(P_WORKLISTID = 6 AND  TRIM ( V . IPSTATUSCODE ) != 'G'))                                                                       
        AND (                                                                  
			P_STARTINGLETTERS IS NULL                                               
			OR                                                                      
			V . LASTNAME >= P_STARTINGLETTERS                                       
			)                                                                       
        AND (                                                                  
			P_ENDINGLETTERS IS NULL                                                  
			OR                                                                      
			V . LASTNAME <= P_ENDINGLETTERS || 'ZZZZZ'                               
			)                                                                       
        AND 
			(                                                                   
			STARTDATE IS NULL                                                        
			OR                                                                       
				(                                                                       
				STARTDATE IS NOT NULL                                                   
				AND ENDDATE IS NOT NULL                                                 
				AND DATE ( V . ADMISSIONDATE ) >= STARTDATE                              
				AND DATE ( V . ADMISSIONDATE ) <= ENDDATE                                
				)                                                                        
			)                                                                        
        AND 
			(                                                                    
			NOT EXISTS                                                              
			(                                                                       
				SELECT WORKLISTITEMID                                                   
				FROM ACCOUNTWORKLISTITEMS W2                                            
				WHERE                                                                     
				(                                                                       
					(W2 . WORKLISTID = 6  OR  W2 . WORKLISTID = 5 )                                                            
					AND W2 . FACILITYID = P_FACILITYID                                      
				AND W2 . ACCOUNTNUMBER = V . ACCOUNTNUMBER                               
				AND P_WORKLISTID != 5                                                   
				AND P_WORKLISTID != 6                                                   
				)                                                                       
			)                                                                       
			)                                                                       
        FOR READ ONLY                                                           
                                                                                
        OPTIMIZE FOR 100 ROWS ;                                                 

        --CALL QSYS/QCMDEXC ('CHGQRYA QRYOPTLIB(PACCESS)',0000000026.00000) ;     
                                                                               
        SELECT CURRENT DATE INTO CURRDATE FROM SYSIBM/SYSDUMMY1 ;                                                            
        --Select rangeindays as  periodSpan                                     
                                                                                
        SELECT RANGEINDAYS                                                      
        INTO PERIODSPAN                                                          
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
