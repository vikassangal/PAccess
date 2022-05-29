                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  LOCATIONMATCHING  - STORED PROCEDURE FOR PX          */        
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
/* 06/28/2006 I  Melissa Bouse     I Modified STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE LOCATIONMATCHING (                                             
        IN P_FACILITYID INTEGER ,                                               
        IN P_ISOCCUPIED VARCHAR(1) ,                                            
        IN P_GENDER VARCHAR(1) ,                                                
        IN P_NURSINGSTATION VARCHAR(5) ,                                        
        IN P_ROOM VARCHAR(5) )                                                  
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC LOCMCH                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        P1 : BEGIN                                                              
                                                                                
        DECLARE CUR_STR VARCHAR ( 15000 ) ;                                     
        DECLARE CUR_WHERE VARCHAR ( 5000 ) ;                                    
        DECLARE CUR_GENDER VARCHAR ( 5000 ) ;                                   
        DECLARE INSQL VARCHAR ( 25000 ) ;                                       
        DECLARE CURSOR1 CURSOR FOR S1 ;                                         
                                                                                
        SET CUR_WHERE = ' ' ;                                                   
        SET CUR_GENDER = ' ' ;                                                  
        IF P_NURSINGSTATION = 'ALL' THEN                                        
           SET P_NURSINGSTATION = NULL ;                                        
        END IF ;                                                                
                                                                                
                        IF P_ROOM = 'ALL' THEN                                  
                        SET P_ROOM = NULL ;                                     
                        END IF ;                                                
                                                                                
                        SET CUR_STR =                                           
                        'SELECT lrp.LRNS as NURSINGSTATION,                     
                        lrp.LRROOM as ROOM   ,                                  
                        lrp.LRBED as BED   ,                                    
                        lrp.LROV as OVERFLOWFLAG ,                              
                        lrp.LRRCON as ROOMCONDITION   ,                         
                        av.FACILITYID   ,                                       
                        av.ACCOUNTNUMBER   ,                                    
                        av.ADMISSIONDATE   ,                                    
                        av.DISCHARGEDATE         ,                              
                        av.SERVICECODE             ,                            
                        av.CLINICCODEID      ,                                  
                        av.CLINICCODE       ,                                   
                        av.CLINICDESCRIPTION       ,                            
                        av.FINANCIALCODE     ,                                  
                        av.FIRSTNAME       ,                                    
                        av.LASTNAME       ,                                     
                        av.MIDDLEINITIAL       ,                                
                        av.PATIENTTYPE   ,                                      
                        av.OPSTATUSCODE       ,                                 
                        av.IPSTATUSCODE       ,                                 
                        av.FINALBILLINGFLAG       ,                             
                        av.OPVISITNUMBER       ,                                
                        av.PENDINGPURGE       ,                                 
                        av.UNBILLEDBALANCE       ,                              
                        av.MEDICALSERVICECODE       ,                           
                        AV.LOCKINDICATOR,                                       
                        AV.LOCKERPBARID,                                        
                        AV.LOCKDATE,                                            
                        AV.LOCKTIME,                                            
                        av.OPERATINGDRID       ,                                
                        av.ATTENDINGDRID       ,                                
                        av.ADMITTINGDRID       ,                                
                        av.REFERINGDRID       ,                                 
                        av.OTHERDRID       ,                                    
                        av.CONSULTINGDR1ID       ,                              
                        av.CONSULTINGDR2ID       ,                              
                        av.CONSULTINGDR3ID       ,                              
                        av.CONSULTINGDR4ID       ,                              
                        av.CONSULTINGDR5ID       ,                              
                        av.NURSINGSTATION        ,                              
                        av.ROOM       ,                                         
                        av.BED        ,                                         
                        av.ISOLATIONCODE       ,                                
                        av.PENDINGDISCHARGE       ,                             
                        av.DISCHARGECODE       ,                                
                        av.VALUABLESARETAKEN       ,                            
                        av.ABSTRACTEXISTS       ,                               
                        av.CLERGYVISIT       ,                                  
                        av.CHIEFCOMPLAINT       ,                               
                        av.ACCOMODATIONCODE       ,                             
                        av.ACCOMODATIONDESC	,
                        av.BLOODLESS       ,                                    
                        av.RELIGIONID      ,                                    
                        av.PLACEOFWORSHIP,                                      
                        av.MEDICALRECORDNUMBER       ,                          
                        av.DOB       ,                                          
                        av.GENDERID       ,                                     
                        lrp.LRAVFG AS UNOCCUPIED       ,                        
                        lrp.lrflg AS PENDINGADMISSION       ,                   
                        av.OPTOUT  AS OPTOUT       ,                            
                        AV.CONFIDENTIAL                                         
                        FROM hpadlrp lrp ' ;                                    
                                                                                
                        IF P_GENDER IS NOT NULL THEN                            
                        SET CUR_STR = CUR_STR || '                              
                        LEFT OUTER JOIN (SELECT facilityid,                     
                        nursingstation,                                         
                        room                                                    
                        FROM accountproxies                                     
                        WHERE facilityid =' || P_FACILITYID ||'                 
                        GROUP BY                                                
                        facilityid, nursingstation, room, genderid              
                        EXCEPT                                                  
                        SELECT                                                  
                        facilityid,nursingstation,room                          
                        from accountproxies                                     
                        WHERE facilityid =' || P_FACILITYID || '                
                        and genderid <>''' || P_GENDER || '''                   
                        group by facilityid,                                    
                        nursingstation,                                         
                        room,                                                   
                        genderid  ) AS AP                                       
                        ON   (LRP.LRHSP#=AP.facilityid                          
                        AND LRP.LRNS=AP.NURSINGSTATION                          
                        AND LRP.LRROOM=AP.ROOM )' ;                             
                                END IF ;                                        
                                                                                
                        SET CUR_STR = CUR_STR || '                              
                        LEFT OUTER JOIN  ACCOUNTPROXIES  av                     
                        ON (lrp.LRHSP# = av.FACILITYID                          
                        AND  LRP.LRACCT=AV.ACCOUNTNUMBER                        
                        and  av.FACILITYID = ' || P_FACILITYID || '             
                        AND TRIM(AV.NursingStation) <> '''')                    
                        WHERE LRP.LRHSP# = ' || P_FACILITYID || '               
                        AND lrp.LRFLG <> ''D''' ;                               
                                                                                
                        IF P_GENDER IS NOT NULL THEN                            
                        SET CUR_STR = CUR_STR || '                              
                        AND (LRP.LRNS || LRP.LRROOM                             
                        IN (AP.NURSINGSTATION||AP.ROOM))' ;                     
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'Y'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NULL THEN                               
                        SET CUR_WHERE = '                                       
                        AND  LRP.LRNS = ''' || P_NURSINGSTATION || '''' ;       
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'Y'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NOT NULL                                  
                        AND P_GENDER IS NULL THEN                               
                        SET CUR_WHERE = '                                       
                        and LRP.LrNS = ''' || P_NURSINGSTATION || '''           
                        and lrp.LRROOM = ''' || P_ROOM || '''' ;                
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NULL                            
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NULL THEN                               
                        SET CUR_WHERE = '                                       
                        AND (LRP.LRNS||LRP.LRROOM                               
                        IN (SELECT DISTINCT LRNS||LRROOM                        
                        FROM HPADLRP                                            
                        WHERE LRHSP# =' || P_FACILITYID || '                    
                        and LRACCT = 0                                          
                        and lrflg='''')                                         
                        OR AV.NURSINGSTATION='''')' ;                           
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NULL THEN                               
                        SET CUR_WHERE = '                                       
                        AND (LRP.LRNS||LRP.LRROOM                               
                        IN (SELECT DISTINCT LRNS||LRROOM                        
                        FROM HPADLRP                                            
                        WHERE LRHSP# =' || P_FACILITYID || '                    
                        and LRACCT = 0                                          
                        and lrflg='''')                                         
                        OR AV.NURSINGSTATION='''')                              
                        AND (LRP.LrNS = ''' || P_NURSINGSTATION || '''  )' ;    
                                END IF ;                                        
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NOT NULL                                  
                        AND P_GENDER IS NULL THEN                               
                        SET CUR_WHERE = '                                       
                        AND (LRP.LRNS||LRP.LRROOM                               
                        IN (SELECT DISTINCT LRNS||LRROOM                        
                        FROM HPADLRP                                            
                        WHERE  LRHSP# =' || P_FACILITYID || '                   
                        and LRACCT = 0 and lrflg='''')                          
                        OR AV.NURSINGSTATION='''')                              
                        AND (LRP.LrNS = ''' || P_NURSINGSTATION || '''          
                        and  lrp.LRROOM= ''' || P_ROOM || ''')' ;               
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'Y'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NOT NULL THEN                           
                        SET CUR_WHERE = '                                       
                        AND  LRP.LRNS = ''' || P_NURSINGSTATION || '''' ;       
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NULL                            
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NOT NULL THEN                           
                        SET CUR_WHERE = 'AND (LRP.LRNS||LRP.LRROOM IN           
                        (SELECT DISTINCT LRNS||LRROOM                           
                        FROM HPADLRP                                            
                        WHERE LRHSP# =' || P_FACILITYID || '                    
                        and LRACCT = 0 and lrflg='''')) ' ;                     
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NULL                                      
                        AND P_GENDER IS NOT NULL THEN                           
                        SET CUR_WHERE = '                                       
                        AND (LRP.LRNS||LRP.LRROOM IN                            
                        (SELECT DISTINCT LRNS||LRROOM                           
                        FROM HPADLRP                                            
                        WHERE  LRHSP#=' || P_FACILITYID || '                    
                        and LRACCT = 0 and lrflg=''''))                         
                        and (LRP.LrNS = ''' || P_NURSINGSTATION || ''' )' ;     
                        END IF ;                                                
                                                                                
                        IF P_ISOCCUPIED = 'N'                                   
                        AND P_NURSINGSTATION IS NOT NULL                        
                        AND P_ROOM IS NOT NULL                                  
                        AND P_GENDER IS NOT NULL THEN                           
                        SET CUR_WHERE = 'AND (LRP.LRNS||LRP.LRROOM IN           
                        (SELECT DISTINCT LRNS||LRROOM                           
                        FROM HPADLRP                                            
                        WHERE  LRHSP#=' || P_FACILITYID || '                    
                        and LRACCT = 0 and lrflg='''') )                        
                        and (LRP.LrNS = ''' || P_NURSINGSTATION || '''          
                        and lrp.LRROOM = ''' || P_ROOM || ''' )' ;              
                        END IF ;                                                
                                                                                
                        SET INSQL = CUR_STR || CUR_WHERE || CUR_GENDER || '     
                        ORDER BY LRP.LrNS,lrp.LRROOM,lrp.LRBED ' ;              
                                                                                
                        PREPARE S1 FROM INSQL ;                                 
                                                                                
                        OPEN CURSOR1 ;                                          
                                                                                
                        END P1  ;                                               
