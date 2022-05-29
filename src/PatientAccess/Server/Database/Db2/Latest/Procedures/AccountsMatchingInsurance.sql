                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCMCHINS  - STORED PROCEDURE FOR PX                 */        
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
/* 07/06/2006 I                    I Modified STORED PROCEDURE        */        
/* 07/18/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ACCOUNTSMATCHINGINSURANCE (                                    
                                                                                
        IN P_COVERAGECATEGORIES VARCHAR(1000) ,                                 
                                                                                
        IN P_NURSINGSTATIONS VARCHAR(1000) ,                                    
                                                                                
        IN P_FACILITYID INTEGER ,                                               
                                                                                
        IN P_HSPCODE VARCHAR(3) )                                               
                                                                                
        DYNAMIC RESULT SETS 2                                                   
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC ACCMCHINS                                                      
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        READS SQL DATA                                                          
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
        DECLARE CUR_STR VARCHAR ( 25000 ) ;                                     
        DECLARE CUR_WHERE VARCHAR ( 5000 ) ;                                    
        DECLARE CUR_WHERE1 VARCHAR ( 5000 ) ;                                   
        DECLARE LHSPCODE VARCHAR ( 100 ) ;                                      
        DECLARE INSQL VARCHAR ( 25000 ) ;                                       
                                                                                
        DECLARE CURSOR1 CURSOR FOR S1 ;                                         
                                                                                
        SET CUR_WHERE = ' ' ;                                                   
        SET CUR_WHERE1 = ' ' ;                                                  
        SET CUR_STR = ' ' ;                                                     
        SET CUR_STR =                                                           
                                                                                
        'SELECT DISTINCT                                                        
        av.FACILITYID,                                                          
        av.ACCOUNTNUMBER,                                                       
        av.ADMISSIONDATE,                                                       
        DAYS(curdate()) - DAYS(av.admissiondate) as LengthOfStay,               
        av.DISCHARGEDATE,                                                       
        hbp.HBINM as payorname,                                                 
        av.SERVICECODE,                                                         
        av.CLINICCODEID,                                                        
        av.CLINICCODE,                                                          
        av.CLINICDESCRIPTION,                                                   
        av.FINANCIALCODE,                                                       
        av.FIRSTNAME,                                                           
        av.LASTNAME,                                                            
        av.MIDDLEINITIAL,                                                       
        av.PATIENTTYPE,                                                         
        av.OPSTATUSCODE,                                                        
        av.IPSTATUSCODE,                                                        
        av.FINALBILLINGFLAG,                                                    
        av.OPVISITNUMBER,                                                       
        av.PENDINGPURGE,                                                        
        av.UNBILLEDBALANCE,                                                     
        av.MEDICALSERVICECODE,                                                  
        av.LOCKINDICATOR,                                                       
        av.LOCKERPBARID,                                                        
        av.LOCKDATE,                                                            
        av.LOCKTIME,                                                            
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
        av.NURSINGSTATION ,                                                     
        av.ROOM,                                                                
        av.BED,                                                                 
        av.ISOLATIONCODE,                                                       
        av.PENDINGDISCHARGE,                                                    
        av.DISCHARGECODE,                                                       
        av.VALUABLESARETAKEN,                                                   
        av.ABSTRACTEXISTS,                                                      
        av.CLERGYVISIT,                                                         
        av.CHIEFCOMPLAINT,                                                      
        av.ACCOMODATIONCODE,                                                    
        ac.QTACCD as ACCOMODATIONDESC,                                          
        av.BLOODLESS,                                                           
        av.RELIGIONID,                                                          
        av.PLACEOFWORSHIP,                                                      
        av.MEDICALRECORDNUMBER,                                                 
        av.DOB,                                                                 
        av.GENDERID,                                                            
        AV.OPTOUT as OptOut,                                                    
        av.Confidential,                                                        
        av.LASTMAINTENANCEDATE,                                                 
        av.LASTMAINTENANCELOGNUMBER,                                            
        av.UPDATELOGNUMBER,                                                     
        pmp.PMPIC5 as PrimaryInsurancePlan,                                     
        pmp.PM2IC5 as SecondaryPlan,                                            
        pmp.PMP#NM as Primaryplanname,                                          
        pmp.PM2#NM as Secondaryplanname,                                        
        hbp.HBCR09 as LiabilityFlag,                                            
        pmp.PMTDU9 as AmountDue,                                                
        pmp.PMTPD9 as Payments,                                                 
        pc.plancategoryid as plancategoryid,                                    
        hbp.HBNUM1 as INSUREDTOTALAMOUNTDUE ,                                   
        hbp.HBNUM2 as UNINSUREDTOTALAMOUNTDUE                                   
        FROM  Accountproxies av                                                 
        left outer join hpadqtac ac                                             
        on  (av.accomodationcode=ac.qtkey                                       
        and av.facilityid=ac.qthsp#)                                            
        left outer join  pm0001p pmp                                            
        ON  (pmp.PMHSPC=''' || P_HSPCODE|| '''                                  
        AND   pmp.PMPT#9 = av.AccountNumber)                                    
        left outer join hpadhbp hbp                                             
        on(hbp.HBHSP#=av.facilityid                                             
        and  hbp.HBACCT=av.AccountNumber)                                       
        left outer join plancategories pc                                       
        on (pc.plancategoryid =                                                 
        GetCategoryType(upper(substr(pmp.PMPIC5, 4, 2))))                       
        where Av.facilityID =' || P_FACILITYID || '                             
        AND ((av.patientType=''1''                                              
        and av.SERVICECODE <> ''LB'')                                           
        or   (av.patientType in(''2'',''4'')                                    
        and av.SERVICECODE in  (''58'',''59'',''LD'')))                         
        AND (av.DISCHARGEDATE is null                                           
        OR DATE(av.DISCHARGEDATE)  = CURRENT DATE                               
        OR av.predischargestatus =''D'')                                        
        AND ( AV.BED <> '''') ' ;                                               
        IF P_COVERAGECATEGORIES <> 'ALL' THEN                                   
        SET CUR_WHERE1 = ' AND (pc.plancategoryid                               
        IN ( ' || P_COVERAGECATEGORIES || ')) ' ;                               
        END IF ;                                                                
                                                                                
        IF P_NURSINGSTATIONS <> 'ALL' THEN                                      
        SET CUR_WHERE = ' AND ( AV.NURSINGSTATION                               
        IN ( ' || P_NURSINGSTATIONS || ')) ' ;                                  
        END IF ;                                                                
                                                                                
        SET INSQL = CUR_STR || CUR_WHERE1 || CUR_WHERE || '                     
        ORDER BY payorname,                                                     
        pmp.PMPIC5,                                                             
        av.LASTNAME ,                                                           
        av.FIRSTNAME,                                                           
        av.MIDDLEINITIAL' ;                                                     
                                                                                
        PREPARE S1 FROM INSQL ;                                                 
                                                                                
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
                                                                                
