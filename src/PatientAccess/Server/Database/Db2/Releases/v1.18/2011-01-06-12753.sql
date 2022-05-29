                                                                                
/**********************************************************************/        
/* AS400 Short Name: PHYACTFOR                                        */        
/* iSeries400        PHYSICIANACCOUNTSFOR -SQL PROC FOR PATIENT ACCESS*/        
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
/***********I**********I************I**********************************/        
/*  Date    I Request #I  Pgmr      I  Modification Description       */        
/***********I**********I************I**********************************/        
/* 05/17/06 I 4043030  I  D Evans   I NEW PROC                        */        
/* 04/16/08 I          I Deepa Raju I OTD# 37191 - Removed Integer    */        
/*          I          I            I conversion for Referring        */        
/*          I          I            I Doctor Id                       */        
/***********I**********I***********I***********************************/        
/* 12/29/2010 I  Deepa Raju        I SR1190 - Read new column to      */        
/*            I                    I   denote Short Registration acct */        
/***********I**********I***********I***********************************/        

SET PATH *LIBL ; 
  
CREATE PROCEDURE PHYSICIANACCOUNTSFOR ( 
	IN P_FACILITYID INTEGER , 
	IN P_PHYSICIANNUMBER INTEGER , 
	IN P_ADMITTING INTEGER , 
	IN P_ATTENDING INTEGER , 
	IN P_REFERRING INTEGER , 
	IN P_CONSULTING INTEGER , 
	IN P_OPERATING INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PHYACTFOR 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
    SET OPTION dbgview = *source                                            

	P1 : BEGIN	
	DECLARE CURSOR_STR VARCHAR ( 25000 ) ;	
	DECLARE CURSOR_WHERE VARCHAR ( 10000 ) ;	
	DECLARE CURSOR_DISCHARGE VARCHAR ( 5000 ) ;	
	DECLARE CUR_STR_PATIENT_TYPE VARCHAR ( 5000 ) ;	
	DECLARE CURSOR_STR_WHERE VARCHAR ( 5000 ) ;	
	DECLARE REFDRID VARCHAR ( 5 ) ;	
	DECLARE CURSOR1 CURSOR FOR S1 ;	

	SET CURSOR_STR_WHERE = '' ;	
	SELECT LPAD ( VARCHAR ( P_PHYSICIANNUMBER ) , 5 , '0' )	
		INTO REFDRID FROM SYSIBM / SYSDUMMY1 ;		

	SET CURSOR_STR = ' SELECT 
		AV.FACILITYID,                                       	
		AV.ACCOUNTNUMBER, 
		AV.ADMISSIONDATE,                                            	
		AV.DISCHARGEDATE,                                                              	
		AV.SERVICECODE,                                                                	
		AV.CLINICCODEID,                                                               	
		AV.CLINICCODE,                                                                 	
		AV.CLINICDESCRIPTION ,                                                         	
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
		AV.SHORTREGTYPE AS SHORTREGISTRATIONTYPE,	
		AV.VALUABLESARETAKEN,                                                          	
		AV.ABSTRACTEXISTS,                                                             	
		AV.CHIEFCOMPLAINT,                                                             	
		AV.ACCOMODATIONCODE,                                                           	
		AV.ACCOMODATIONDESC,                                                           	
		AV.MEDICALRECORDNUMBER,               	
		AV.DOB,                                                                        	
		AV.GENDERID,                                                                   	
		AV.OPTOUT AS OPTOUT,                                                           	
		AV.CONFIDENTIAL,                                                               	
		AV.LASTMAINTENANCEDATE,                                                        	
		AV.LASTMAINTENANCELOGNUMBER,                                                   	
		AV.UPDATELOGNUMBER                                                             	
	
	FROM ACCOUNTPROXIES AV                                                         	
	WHERE FACILITYID=' || P_FACILITYID ;		
	
	SET CURSOR_DISCHARGE = ' ((AV.DISCHARGEDATE IS NULL)                           	
		OR   (DATE(AV.DISCHARGEDATE) = CURRENT_DATE)                                   	
		OR (AV.PENDINGDISCHARGE = ''Y'' )) ' ;		
	
	SET CUR_STR_PATIENT_TYPE = ' ( AV.PATIENTTYPE IN                               					
		(''1'',''2'',''3'',''9'')                                                  					
		OR (AV.PATIENTTYPE = ''4''                                                 					
		AND (DATE(AV.ADMISSIONDATE) =                                              					
		CURRENT_DATE  OR AV.BED<>''''))) ' ;		 
	--/* suggestion : try using an array to store where conditions  */ --          
		
	IF P_ADMITTING = 1 THEN		
	SET CURSOR_STR_WHERE = CURSOR_STR_WHERE ||	
		'OR  (ADMITTINGDRID=' || P_PHYSICIANNUMBER || ')' ;		
	END IF ;			
	
	IF P_ATTENDING = 1 THEN		
	SET CURSOR_STR_WHERE = CURSOR_STR_WHERE ||	
		'OR  (ATTENDINGDRID=' || P_PHYSICIANNUMBER || ')' ;		
	END IF ;			
	
	IF P_REFERRING = 1 THEN		
	SET CURSOR_STR_WHERE = CURSOR_STR_WHERE ||	
		'OR  (REFERINGDRID=''' || REFDRID || ''')' ;		
	END IF ;			
	
	IF P_OPERATING = 1 THEN		
	SET CURSOR_STR_WHERE = CURSOR_STR_WHERE ||	
		'OR  (OPERATINGDRID=' || P_PHYSICIANNUMBER || ')' ;		
	END IF ;			
	
	IF P_CONSULTING = 1 THEN		
	SET CURSOR_STR_WHERE = CURSOR_STR_WHERE || 
		'OR ( CONSULTINGDR1ID=' ||	P_PHYSICIANNUMBER || 
		' OR CONSULTINGDR2ID=' ||	P_PHYSICIANNUMBER || 
		' OR CONSULTINGDR3ID=' ||	P_PHYSICIANNUMBER || 
		' OR CONSULTINGDR4ID=' ||	P_PHYSICIANNUMBER || 
		' OR CONSULTINGDR5ID=' ||	P_PHYSICIANNUMBER || ')' ;		
	END IF ;		
	
	IF LENGTH ( CURSOR_STR_WHERE ) > 0 THEN		
	SET CURSOR_STR = CURSOR_STR ||	
		' AND ( ' || SUBSTR ( CURSOR_STR_WHERE , 4 ,	
		LENGTH ( CURSOR_STR_WHERE ) ) || ' )' ||	
		' AND ' || CURSOR_DISCHARGE ||	
		' AND ' || CUR_STR_PATIENT_TYPE ||	
		' ORDER BY AV.NURSINGSTATION, AV.ROOM, AV.BED, AV.LASTNAME, AV.FIRSTNAME ' ;		
	
	ELSE		
	SET CURSOR_STR = CURSOR_STR ||	
		' AND ' || CURSOR_DISCHARGE ||	
		' AND ' || CUR_STR_PATIENT_TYPE ||	
		' ORDER BY AV.NURSINGSTATION, AV.ROOM, AV.BED, AV.LASTNAME,AV.FIRSTNAME ' ;		
	
	END IF ;		
	
	PREPARE S1 FROM CURSOR_STR ;		
	OPEN CURSOR1 ;		
	END P1  ;

