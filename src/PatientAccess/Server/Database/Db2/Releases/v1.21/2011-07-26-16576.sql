																				
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  BLOODLESSTREATMENTACCOUNTSFOR - SQL PROC FOR PX      */        
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
/* 12/29/2010 I  Deepa Raju        I SR1190 - Read new column to      */        
/*            I                    I   denote Short Registration acct */      
/* 07/26/2011 I  Nereid Tavares    I SR1557 - Read new column to      */        
/*            I                    I   denote Newborn Reg acct        */       
/*************I********************I***********************************/        
																				
SET PATH *LIBL ;                                                                
																				
CREATE PROCEDURE BLOODLESSTREATMENTACCOUNTSFOR (                                
		IN P_FACILITYID INTEGER ,                                               
		IN P_PATIENTTYPE VARCHAR(1) ,                                           
		IN P_ADMITDATE VARCHAR(1) )                                             
		DYNAMIC RESULT SETS 1                                                   
		LANGUAGE SQL                                                            
		SPECIFIC BLDTRGACT                                                      
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
		DECLARE CUR_STR VARCHAR ( 10000 ) ; 
		DECLARE CUR_WHERE VARCHAR ( 10000 ) ; 
		DECLARE CURSOR1 CURSOR FOR S1 ; 
		
		SET CUR_STR = ' SELECT                                                            
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
		AV.NURSINGSTATION,                                                               
		AV.ROOM, 
		AV.BED,                                                                         
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
		ac.qtaccd as AccomodationDesc,                                                   
		AV.BLOODLESS,                                                                   
		AV.RELIGIONID,                                                                  
		AV.PLACEOFWORSHIP,                                        
		AV.MEDICALRECORDNUMBER,                                                         
		AV.DOB,                                                                         
		AV.GENDERID,                                                                     
		av.OPTOUT  AS OPTOUT,                                                          
		av.CONFIDENTIAL,                                                                
		AV.LASTMAINTENANCEDATE,                    
		AV.LASTMAINTENANCELOGNUMBER,                                                    
		AV.UPDATELOGNUMBER  
		
		FROM ACCOUNTPROXIES AV                                    
		left outer join hpadqtac ac                                                      
			on (av.ACCOMODATIONCODE= ac.qtkey                                                      
			and av.facilityid=ac.qthsp#)                                               
		WHERE AV.FACILITYID =' || P_FACILITYID || '  
			AND UPPER(AV.BLOODLESS) =''Y''                                                  
			AND (DATE(AV.DISCHARGEDATE) = CURRENT DATE  
			OR  AV.DISCHARGEDATE IS NULL                                                        
			OR AV.PENDINGDISCHARGE = ''Y'')' ; 
		
		IF UPPER ( TRIM ( P_PATIENTTYPE ) ) = 'A' 
			AND UPPER ( TRIM ( P_ADMITDATE ) ) = 'A' 
		THEN SET CUR_WHERE = ' AND (AV.PATIENTTYPE IN                                                
			(''0'',  ''1'',''2'',''3'',''9'')                       
			OR ((AV.PATIENTTYPE = ''4'')    AND                                                     
			(DATE(AV.ADMISSIONDATE) = CURRENT DATE                                  
			OR AV.BED<>'''')))' ; 
		SET CUR_STR = CUR_STR || CUR_WHERE || ' 
			ORDER BY LASTNAME, FIRSTNAME, MIDDLEINITIAL' ; 
		END IF ; 
		
		IF UPPER ( TRIM ( P_PATIENTTYPE ) ) = 'P' 
			AND UPPER ( TRIM ( P_ADMITDATE ) ) = 'A' 
		THEN SET CUR_WHERE = ' AND AV.PATIENTTYPE =''0''' ; 
		SET CUR_STR = CUR_STR || CUR_WHERE || ' 
			ORDER BY LASTNAME, FIRSTNAME, MIDDLEINITIAL' ; 
		END IF ; 
		
		IF UPPER ( TRIM ( P_PATIENTTYPE ) ) = 'R' 
			AND UPPER ( TRIM ( P_ADMITDATE ) ) = 'A' 
		THEN SET CUR_WHERE = ' AND ( AV.PATIENTTYPE IN                                               
			(''1'',''2'',''3'',''9'')                                               
			OR ((AV.PATIENTTYPE = ''4'')    AND                                                      
			(DATE(AV.ADMISSIONDATE) = CURRENT DATE                                 
			OR AV.BED<>'''')))' ; 
		SET CUR_STR = CUR_STR || CUR_WHERE || ' 
			ORDER BY LASTNAME, FIRSTNAME, MIDDLEINITIAL' ; 
		END IF ; 
		
		IF UPPER ( TRIM ( P_PATIENTTYPE ) ) = 'R' 
			AND UPPER ( TRIM ( P_ADMITDATE ) ) = 'T' 
		THEN SET CUR_WHERE = ' AND ( DATE(AV.ADMISSIONDATE )                                         
			= CURRENT DATE )                                                        
			AND (AV.PATIENTTYPE IN (''1'',''2'',''3'',''9'')                                
			OR (AV.PATIENTTYPE = ''4''))' ; 
		SET CUR_STR = CUR_STR || CUR_WHERE || ' 
			ORDER BY LASTNAME, FIRSTNAME, MIDDLEINITIAL' ; 
		END IF ; 
		
		PREPARE S1 FROM CUR_STR ; 
		OPEN CURSOR1 ; 
		END P1  ;

