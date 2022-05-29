                                                                                
/**********************************************************************/        
/* AS400 Short Name: RELIG00002                                       */        
/* iSeries400        RELIGIONSUMMARYFOR -SQL PROC FOR PATIENT ACCESS  */        
/*                                                                    */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *                                                          *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods and techniques described herein are          *    */        
/*    * considered trade secrets and/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*                                                                    */        
/*                                                                    */        
/***********I**********I***********I***********************************/        
/*  Date    I Request #I  Pgmr     I  Modification Description        */        
/***********I**********I***********I***********************************/        
/* 05/17/06 I 4043030  I  D Evans  I NEW PROC                         */        
/***********I**********I***********I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE RELIGIONSUMMARYFOR (                                           
        IN P_FACILITYID INTEGER ,                                               
        IN P_RELIGIONCODE VARCHAR(15) )                                         
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC RGNSUMFOR                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
        DECLARE CUR_STR VARCHAR ( 5000 ) ;                                      
        DECLARE CUR_WHERE VARCHAR ( 2500 ) ;                                    
        DECLARE CUR_GENDER VARCHAR ( 2500 ) ;                                   
        DECLARE INSQL VARCHAR ( 10000 ) ;                                       
        DECLARE CURSOR1 CURSOR FOR S1 ;                                         
                                                                                
        SET CUR_WHERE = ' ' ;                                                   
        SET CUR_GENDER = ' ' ;                                                  
                                                                                
        SET CUR_STR = 'SELECT R.QTRELD AS ReligionDescription,                  
         COUNT(AV.RELIGIONID) AS ReligionTotal                                  
         FROM Accountproxies av                                                 
        left outer  join HPADQTRL r on(r.QTKEY=av.religionid                    
        and r.qthsp#=av.facilityid)                                             
          WHERE av.FacilityID = ' || P_FACILITYID || '   AND                    
        (av.patientType=''1'')                                                  
        AND                                                                     
         (av.DISCHARGEDATE is null OR date(av.DISCHARGEDATE) =                  
                CURRENT DATE OR av.PENDINGDISCHARGE = ''Y'')' ;                 
                                                                                
        IF P_RELIGIONCODE != 'UNSPECIFIED' THEN                                 
        SET CUR_WHERE =                                                         
        ' AND TRIM(UPPER(AV.RELIGIONID)) = trim(upper(''' ||                    
        P_RELIGIONCODE || '''))' ;                                              
        ELSE                                                                    
        SET CUR_WHERE = ' AND AV.religionid='''' ' ;                            
        END IF ;                                                                
        IF P_RELIGIONCODE = 'ALL' THEN                                          
        SET CUR_WHERE = '' ;                                                    
        END IF ;                                                                
                                                                                
        SET INSQL = CUR_STR || CUR_WHERE ||                                     
        '  GROUP BY AV.RELIGIONID,R.QTRELD ORDER BY R.QTRELD ' ;                
        PREPARE S1 FROM INSQL ;                                                 
                                                                                
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
