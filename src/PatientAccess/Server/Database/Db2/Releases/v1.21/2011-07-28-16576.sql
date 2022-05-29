/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  RELIGIONACCOUNTSFOR - SQL PROC FOR PX                */        
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
/* 12/29/2010 I  Deepa Raju        I SR1190 - Read new column to      */        
/*            I                    I   denote Short Registration acct */   
/* 07/20/2011 I  Nereid Tavares    I SR1557 - Read new column to      */        
/*            I                    I   denote Newborn Reg acct        */       
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE RELIGIONACCOUNTSFOR (                                          
        IN P_FACILITYID INTEGER ,                                               
        IN P_RELIGIONCODE VARCHAR(15) )                                         
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC RGNACCFOR                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION  ALWBLK = *ALLREAD , 
		ALWCPYDTA = *OPTIMIZE , 
		COMMIT = *NONE , 
		DBGVIEW = *SOURCE , 
		CLOSQLCSR = *ENDMOD , 
		DECRESULT = (31, 31, 00) , 
		DFTRDBCOL = *NONE , 
		DLYPRP = *NO , 
		DYNDFTCOL = *NO , 
		DYNUSRPRF = *USER , 
		RDBCNNMTH = *RUW , 
		SRTSEQ = *HEX                                     
		P1 : BEGIN 
		DECLARE CUR_STR VARCHAR ( 25000 ) ; 
		DECLARE CUR_WHERE VARCHAR ( 5000 ) ; 
		DECLARE CUR_GENDER VARCHAR ( 5000 ) ; 
		DECLARE INSQL VARCHAR ( 25000 ) ; 
		DECLARE CURSOR1 CURSOR FOR S1 ; 
		
		SET CUR_WHERE = ' ' ; 
		SET CUR_GENDER = ' ' ; 
		SET CUR_STR = 'SELECT av.FACILITYID,                                                            
		av.ACCOUNTNUMBER,                                                               
		av.ADMISSIONDATE,                                                               
		av.DISCHARGEDATE,                                                               
		av.SERVICECODE,                                                                 
		AV.CLINICCODEID,                                                                
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
		av.NURSINGSTATION,                                                  
		av.ROOM,                                                                        
		av.BED,                                                                         
		av.ISOLATIONCODE,                                                               
		av.PENDINGDISCHARGE,                                                            
		av.DISCHARGECODE,                                                               
		AV.SHORTREGTYPE AS SHORTREGISTRATIONTYPE ,      
		AV.NEWBORNREGTYPE AS NEWBORNREGISTRATIONTYPE ,
		av.VALUABLESARETAKEN,                              
		av.ABSTRACTEXISTS,                                                              
		av.CLERGYVISIT,                                                                 
		av.CHIEFCOMPLAINT,                                                              
		av.ACCOMODATIONCODE,                                                            
		av.ACCOMODATIONDESC,                                                            
		av.BLOODLESS,          
		av.RELIGIONID,                                                                  
		av.PLACEOFWORSHIP,                                                              
		av.MEDICALRECORDNUMBER,                                                         
		av.DOB,                                                                         
		av.GENDERID,                                                                    
		av.optout as OptOut,                                                            
		av.Confidential,                                                                
		av.LASTMAINTENANCEDATE,                                                         
		av.LASTMAINTENANCELOGNUMBER,                                                    
		av.UPDATELOGNUMBER                                                              
		
		FROM Accountproxies av                                                          
		WHERE                                                                           
			av.FacilityID = ' || P_FACILITYID || '                                          
			AND (av.patientType=''1''                                                       
			or (av.patientType in(''2'',''4'')                                              
			and av.SERVICECODE in (''57'',''58'',''59'',''LD''))                            
			or (av.patientType=''9'' and av.SERVICECODE= ''LB''))                           
			AND (av.DISCHARGEDATE is null                                                   
			OR DATE(av.DISCHARGEDATE)= CURRENT DATE                              
			OR av.PENDINGDISCHARGE = ''Y'')' ; 
		
		IF P_RELIGIONCODE != 'UNSPECIFIED' 
		THEN SET CUR_WHERE = '                                                                               
			AND TRIM(UPPER(AV.RELIGIONID)) =                                                
			trim(upper(''' || P_RELIGIONCODE || '''))' ; 
		ELSE SET CUR_WHERE = ' AND AV.religionid='''' ' ; 
		END IF ; 
		
		IF P_RELIGIONCODE = 'ALL' 
		THEN SET CUR_WHERE = '' ; 
		END IF ; 
		
		SET INSQL = CUR_STR || CUR_WHERE || '                                          
		ORDER BY                                                                        
		RELIGIONID,                                                                     
		LASTNAME,                                                                       
		FIRSTNAME ,                                                                     
		MIDDLEINITIAL ' ; 
		
		PREPARE S1 FROM INSQL ; 
		OPEN CURSOR1 ; 
		END P1  ;

