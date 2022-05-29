                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCMCH  - STORED PROCEDURE FOR PX                    */        
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
/* 04/26/2006 I  Melissa Bouse     I STORED PROCEDURE                 */        
/*************I********************I***********************************/        
/* 06/26/2006 I  Melissa Bouse     I STORED PROCEDURE                 */        
/* 12/29/2010 I  Deepa Raju        I SR1190 - Read new column to      */        
/*            I                    I   denote Short Registration acct */  
/* 07/23/2011 I  Nereid Tavares    I SR1557 - Read new column to      */        
/*            I                    I   denote Newborn Reg acct        */            
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ACCOUNTSMATCHING (                                             
        IN P_ADTACTIVITY VARCHAR(1) ,                                           
        IN P_STARTTIME VARCHAR(4) ,                                             
        IN P_NURSINGSTATIONS VARCHAR(1000) ,                                    
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ACCMCH                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
		SET OPTION  ALWBLK = *ALLREAD , 
		ALWCPYDTA = *OPTIMIZE , 
		COMMIT = *NONE , 
		CLOSQLCSR = *ENDMOD , 
		DECRESULT = (31, 31, 00) , 
		DFTRDBCOL = *NONE , 
		DLYPRP = *NO , 
		DYNDFTCOL = *NO , 
		DYNUSRPRF = *USER , 
		RDBCNNMTH = *RUW , 
		SRTSEQ = *HEX   
		P1 : BEGIN DECLARE STMT VARCHAR ( 25000 ) ; 
				DECLARE STMT_WHERE VARCHAR ( 10000 ) ; 
				DECLARE CURSOR1 CURSOR FOR S1 ; 
				SET STMT_WHERE = ' ' ; 
				SET STMT = '' ; 
				SET STMT = '    
					
		SELECT                                                                          
		AP.AANS AS NURSINGSTATIONFROM,                                                  
		AP.AAROOM AS ROOMFROM,                                                          
		AP.AABED AS BEDFROM,                                                            
		AP.AATNS AS NURSINGSTATIONTO,                                                   
		AP.AATRM AS ROOMTO,                                                             
		AP.AATBED AS BEDTO,                                                             
		AV.FACILITYID,                                                                  
		AV.ACCOUNTNUMBER,                                                               
		AV.ADMISSIONDATE,                                           
		AV.DISCHARGEDATE,                                                              
		AV.SERVICECODE,                                                                 
		AV.CLINICCODEID,                                                                
		AV.CLINICCODE,                                                                  
		AV.CLINICDESCRIPTION,                                                           
		AV.FINANCIALCODE,                       
		AV.FIRSTNAME,                                                                   
		AV.LASTNAME,                                                                    
		AV.MIDDLEINITIAL,                                                               
		AV.PATIENTTYPE,                                                                 
		AV.OPSTATUSCODE,                                                                
		AV.IPSTATUSCODE,   
		AV.FINALBILLINGFLAG,                                                            
		AV.OPVISITNUMBER,                                                               
		AV.PENDINGPURGE,                                                                
		AV.UNBILLEDBALANCE,                                                             
		AV.MEDICALSERVICECODE,                                                          
		AV.LOCKINDICATOR,                                                               
		AV.LOCKERPBARID,                                                                
		AV.LOCKDATE,                                                                    
		AV.LOCKTIME,                                                                    
		AV.OPERATINGDRID,                                                               
		AV.ATTENDINGDRID,                                                               
		AV.ADMITTINGDRID,                                                               
		AV.REFERINGDRID,                                           
		AV.OTHERDRID,                                                                   
		AV.CONSULTINGDR1ID,                                                             
		AV.CONSULTINGDR2ID,                                                             
		AV.CONSULTINGDR3ID,                                                             
		AV.CONSULTINGDR4ID,                                                             
		AV.CONSULTINGDR5ID,                       
		AV.NURSINGSTATION ,                                                             
		AV.ROOM ,                                                                       
		AV.BED ,                                                                        
		AV.ISOLATIONCODE,                                                               
		AV.PENDINGDISCHARGE,                                                            
		AV.DISCHARGECODE,   
		AV.SHORTREGTYPE AS SHORTREGISTRATIONTYPE ,      
		AV.NEWBORNREGTYPE AS NEWBORNREGISTRATIONTYPE ,
		AV.VALUABLESARETAKEN,                                                           
		AV.ABSTRACTEXISTS,                                                              
		AV.CLERGYVISIT,                                                                 
		AV.CHIEFCOMPLAINT,                                                              
		AV.ACCOMODATIONCODE,                                                            
		AV.ACCOMODATIONDESC,                                                            
		AV.BLOODLESS,                                                               
		AV.RELIGIONID,                                                                  
		AV.PLACEOFWORSHIP,                                                              
		AV.MEDICALRECORDNUMBER,                                                         
		AV.DOB,                                                                         
		AV.GENDERID,                                                                    
		AV.OPTOUT  AS OPTOUT,                                           
		AV.CONFIDENTIAL,                                                                
		AV.LASTMAINTENANCEDATE,                                                         
		AV.LASTMAINTENANCELOGNUMBER,                                                    
		AV.UPDATELOGNUMBER,                                                             
		AP.AAID AS TRANSACTIONTYPE,                                                     
		AP.AATIME AS ADTTIME                        
		FROM HPADAAP AP                                                                
		LEFT OUTER JOIN ACCOUNTPROXIES AV ON                                            
		( AV.FACILITYID = ' || VARCHAR ( P_FACILITYID ) || '                            
			AND AP.AAHSP# = AV.FACILITYID                                                   
			AND AP.AAACCT =   AV.ACCOUNTNUMBER )                                            
		WHERE (AV.FACILITYID = ' || CHAR ( P_FACILITYID ) || ')   
			AND AP.AATIME >=' || VARCHAR ( P_STARTTIME ) ; 

		IF P_ADTACTIVITY = 'A' 
			THEN SET STMT_WHERE = STMT_WHERE || '                                                   
			AND  TRIM(AP.AAID) =''A''                                                       
			AND AV.PATIENTTYPE=''1''' ; 
		END IF ; 
		
		IF P_ADTACTIVITY = 'D' 
			THEN SET STMT_WHERE = STMT_WHERE || '                                                   
			AND  TRIM(AP.AAID)  =''D''                                                      
			AND AV.PATIENTTYPE=''1''' ; 
		END IF ; 
		
		IF P_ADTACTIVITY = 'T' 
			THEN SET STMT_WHERE = STMT_WHERE || '                                                   
			AND TRIM(AP.AAID) =''T''                                                        
			AND (AV.PATIENTTYPE=''1''                                                       
			OR  (AV.PATIENTTYPE IN  (''2'',''4'')                                           
			AND  AV.SERVICECODE IN (''57'',''58'',''59'',''LD''))                           
			OR (AV.PATIENTTYPE=''9'' AND AV.SERVICECODE=''LB''))' ; 
		END IF ; 
		
		IF P_ADTACTIVITY = 'E' 
			THEN SET STMT_WHERE = STMT_WHERE || '                                                   
			AND TRIM(AP.AAID) IN (''A'',''D'',''T'')' ; 
		END IF ; 
		
		IF TRIM ( UPPER ( P_NURSINGSTATIONS ) ) <> 'ALL' 
			THEN SET STMT_WHERE = STMT_WHERE || '                                                   
			AND AP.AANS  IN  (' || P_NURSINGSTATIONS || ')' ; 
		END IF ; 
		
		SET STMT = STMT || STMT_WHERE ;  -- || ' ORDER BY AP.A 
		
		PREPARE S1 FROM STMT ; 
		OPEN CURSOR1 ; 
		END P1  ;
