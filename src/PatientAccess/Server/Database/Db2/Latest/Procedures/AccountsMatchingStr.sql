                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCOUNTSMATCHINGSTR - SQL PROC FOR PX                */        
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
/* 04/27/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ACCOUNTSMATCHINGSTR (                                          
        IN P_ADTACTIVITY VARCHAR(12) ,                                          
        IN P_STARTTIME VARCHAR(12) ,                                            
        IN P_NURSINGSTATIONS VARCHAR(12) ,                                      
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ACCMCHSTR                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CUR_STR VARCHAR ( 10000 ) ;                                     
        DECLARE CUR_WHERE VARCHAR ( 500 ) ;                                     
        DECLARE FMT_TIME VARCHAR ( 4 ) ;                                        
        DECLARE INSQL VARCHAR ( 10000 ) ;                                       
        DECLARE CURSOR1 CURSOR FOR S1 ;                                         
                                                                                
        SET CUR_STR = ' ' ;                                                     
        SET CUR_WHERE = ' ' ;                                                   
        SET FMT_TIME = LPAD ( P_STARTTIME , 4 , '0' ) ;                         
                                                                                
        SET CUR_STR = 'SELECT                                                   
        ap.AANS as NURSINGSTATIONFrom,                                          
        ap.AAROOM as ROOMFrom,                                                  
        ap.AABED as BEDFrom,                                                    
        ap.AATNS as NURSINGSTATIONTo,                                           
        ap.AATRM as ROOMTo,                                                     
        ap.AATBED as BEDTo,                                                     
        av.FACILITYID,                                                          
        av.ACCOUNTNUMBER,                                                       
        av.ADMISSIONDATE,                                                       
        av.DISCHARGEDATE,                                                       
        av.SERVICEID,                                                           
        av.SERVICECODE,                                                         
        av.SERVICEDESCRIPTION,                                                  
        av.CLINICCODEID,                                                        
        av.CLINICCODE,                                                          
        av.CLINICDESCRIPTION,                                                   
        av.FINANCIALCODE,                                                       
        av.FIRSTNAME,                                                           
        av.LASTNAME,                                                            
        av.MIDDLEINITIAL,                                                       
--      av.PATIENTTYPECODE,                                                     
        av.PATIENTTYPEDESC,                                                     
        av.OPSTATUSCODE,                                                        
        av.IPSTATUSCODE,                                                        
        av.FINALBILLINGFLAG,                                                    
        av.OPVISITNUMBER,                                                       
        av.PENDINGPURGE,                                                        
        av.UNBILLEDBALANCE,                                                     
        av.MEDICALSERVICECODE,                                                  
        av.LOCKINDICATOR,                                                       
        av.OPERATINGDRID,                                                       
        av.ATTENDINGDRID,                                                       
        av.ADMITTINGDRID,                                                       
        av.REFERINGDRID,                                                        
        av.OTHERDRID,                                                           
        av.CONSULTINGDR1ID,                                                     
        av.CONSULTINGDR2ID,                                                     
        av.CONSULTINGDR3ID,                                                     
        av.CONSULTINGDR4ID,                                                     
        av.CONSULTINGDR5ID,                                                     
        av.NURSINGSTATION,                                                      
        av.ROOM ,                                                               
        av.BED ,                                                                
        av.ISOLATIONCODE,                                                       
        av.PENDINGDISCHARGE,                                                    
        av.DISCHARGECODE,                                                       
        av.VALUABLESARETAKEN,                                                   
        av.ABSTRACTEXISTS,                                                      
        av.CLERGYVISIT,                                                         
        av.CHIEFCOMPLAINT,                                                      
        av.ACCOMODATIONCODE,    
        av.ACCOMODATIONDESC,                                                
--      av.PATIENTID,                                                           
        av.BLOODLESS,                                                           
        av.RELIGIONID,                                                          
--      av.PLACEOFWORSHIPID,                                                    
        av.MEDICALRECORDNUMBER,                                                 
        av.DOB,                                                                 
        av.GENDERID,                                                            
        DMDP.MDNPPF as OptOut,                                                  
        RRWP.RWCNFG as Confidential,                                            
        AP.AAID AS TransactionType,                                             
        Disp.codedescription as CodeDescription,                                
        ap.aatime as TransactionTime                                            
        FROM HPADAAP AP                                                         
        LEFT JOIN Accountproxies av ON                                          
        (Ap.AAhsp# = av.facilityid AND                                          
        AP.AAACCT = av.AccountNumber)                                           
        LEFT JOIN DischargeDispositionCodes Disp                                
        ON (disp.dispositioncode = av.dischargecode)                            
        LEFT JOIN HPADMDP dmdp                                                  
        on (dmdp.mdhsp# = av.facilityid and                                     
        dmdp.mdmrc# = av.medicalrecordnumber)                                   
        LEFT JOIN HXMRRWP rrwp                                                  
        on (rrwp.rwhsp# = av.facilityid and                                     
        rrwp.rwacct = av.accountnumber)                                         
        WHERE Ap.AAhsp# = ' || CHAR (P_FACILITYID ) || '                        
        and char(ap.aatime) >=                                                  
        char(' || CHAR ( FMT_TIME ) || ')' ;                                    
                                                                                
        IF P_NURSINGSTATIONS IS NOT NULL                                        
        THEN SET CUR_WHERE = '                                                  
        AND ap.aans in (' || P_NURSINGSTATIONS || ')' ;                         
        END IF ;                                                                
                                                                                
        IF P_ADTACTIVITY = 'A'                                                  
        THEN SET CUR_WHERE = CUR_WHERE || '                                     
        AND  AP.AAID =''A''' ;                                                  
        -- AND AV.PATIENTTYPECODE=''1''';                                       
        END IF ;                                                                
                                                                                
        IF P_ADTACTIVITY = 'D'                                                  
        THEN SET CUR_WHERE = CUR_WHERE || '                                     
        AND  AP.AAID =''D''                                                     
        AND AV.PATIENTTYPECODE=''1''' ;                                         
        END IF ;                                                                
                                                                                
        IF P_ADTACTIVITY = 'T'                                                  
        THEN SET CUR_WHERE = CUR_WHERE || '                                     
        AND AP.AAID =''T''' ;                                                   
        END IF ;                                                                
                                                                                
        IF P_ADTACTIVITY = 'E'                                                  
        THEN SET CUR_WHERE = CUR_WHERE || '                                     
        AND AP.AAID in (''A'',''D'',''T'')' ;                                   
        END IF ;                                                                
                                                                                
        SET INSQL = CUR_STR || CUR_WHERE || '                                   
        ORDER BY  ap.aatime' ;                                                  
                                                                                
        PREPARE S1 FROM INSQL ;                                                 
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
