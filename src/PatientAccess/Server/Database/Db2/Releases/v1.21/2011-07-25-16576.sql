                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCOUNTSMATCHINGNURSINGSTATIONS - SQL PROC FOR PX    */        
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
/* 02/28/2008 I  KJS               I remove CHGQRYA call              */       
/* 12/29/2010 I  Deepa Raju        I SR1190 - Read new column to      */        
/*            I                    I   denote Short Registration acct */       
/* 07/25/2011 I  Nereid Tavares    I SR1557 - Read new column to      */        
/*            I                    I   denote Newborn Reg acct        */       
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ACCOUNTSMATCHINGNURSINGSTATIONS (                              
	IN P_ISOCCUPIED CHAR(1) ,                                                    
	IN P_NURSINGSTATION VARCHAR(10) ,                                            
	IN P_FACILITYID INTEGER )                                                    
	DYNAMIC RESULT SETS 2                                                        
	LANGUAGE SQL                                                                 
	SPECIFIC ACCMCHNST                                                           
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
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX  
	P1 : BEGIN 
	
	DECLARE CUR_STR VARCHAR ( 10000 ) ; 
	DECLARE CUR_WHERE VARCHAR ( 500 ) ; 
	DECLARE LHSPCODE VARCHAR ( 100 ) ; 
	DECLARE INSQL VARCHAR ( 10000 ) ; 
	DECLARE CURSOR1 CURSOR FOR S1 ; 
	
	SET CUR_WHERE = ' ' ; 
	SET CUR_STR = ' ' ; 
	SET LHSPCODE = ' ' ; 
	SET CUR_STR = 
		'SELECT lrp.LRNS AS NURSINGSTATION,                               
		lrp.LRROOM AS ROOM,                                                             
		lrp.LRBED AS BED,                                                               
		lrp.LROV AS OVERFLOWFLAG,                                                       
		lrp.LRRCON AS ROOMCONDITION,                                                    
		av.FACILITYID,                                                                  
		av.ACCOUNTNUMBER,                                                               
		av.ADMISSIONDATE,                                                               
		av.DISCHARGEDATE,                                                               
		av.SERVICECODE,                                                
		av.CLINICCODEID,                                                                
		av.CLINICCODE,                                                                  
		av.CLINICDESCRIPTION,                                                           
		av.FINANCIALCODE,                                                               
		av.FIRSTNAME,                                                                   
		av.LASTNAME,                            
		av.MIDDLEINITIAL,                                                               
		av.PATIENTTYPE,                                                                 
		av.medicalservicecode,                                                          
		av.OPSTATUSCODE,                                                                
		av.IPSTATUSCODE,                                                                
		av.FINALBILLINGFLAG,        
		av.OPVISITNUMBER,                                                               
		av.PENDINGPURGE,                                                                
		av.UNBILLEDBALANCE,                                                             
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
		av.NURSINGSTATION AS ns,                                                        
		av.ROOM AS rm,                            
		av.BED AS bd,                                                                   
		av.ISOLATIONCODE,                                                               
		av.PENDINGDISCHARGE,                                                            
		av.DISCHARGECODE,                                                               
		AV.SHORTREGTYPE AS SHORTREGISTRATIONTYPE ,    
		av.NEWBORNREGTYPE AS NEWBORNREGISTRATIONTYPE ,  
		av.VALUABLESARETAKEN,                                                           
		av.ABSTRACTEXISTS,        
		av.CLERGYVISIT,                                                                 
		av.CHIEFCOMPLAINT,                                                              
		av.ACCOMODATIONCODE,                                                            
		av.ACCOMODATIONDESC,                                                            
		av.BLOODLESS,                                                                   
		av.RELIGIONID,                                                                  
		av.MEDICALRECORDNUMBER,                                                         
		av.DOB,                                                                         
		av.GENDERID,                                                                    
		lrp.LRAVFG AS UNOCCUPIED,                                                       
		lrp.lrflg AS PENDINGADMISSION,                                                  
		AV.OPTOUT AS OPTOUT,                                                            
		AV.CONFIDENTIAL,                                                
		av.LASTMAINTENANCEDATE,                                                         
		av.LASTMAINTENANCELOGNUMBER,                                                    
		av.UPDATELOGNUMBER                                                              
		
		FROM HPADLRP lrp                                                                
		LEFT OUTER JOIN  ACCOUNTPROXIES AV 
			ON ( av.facilityid=' || P_FACILITYID || '                            
			AND LRP.LRHSP# = AV.FACILITYID                                                  
			AND LRP.LRACCT = AV.ACCOUNTNUMBER  )                                            
		WHERE  (LRP.LRHSP# =' || P_FACILITYID || ')                                     
			AND lrp.LRFLG <> ''D''                                                          
			AND ( ( AV . DISCHARGEDATE IS NULL )                                            
			OR DATE ( AV . DISCHARGEDATE )= CURRENT DATE)' ; 
		
		IF P_ISOCCUPIED = 'Y' 
			AND TRIM ( UPPER ( P_NURSINGSTATION ) ) = 'ALL' 
		THEN  -- All Nursing Station and Bed Type = 'All'                             
			--SET CUR_STR = CUR_STR ;                                               
		SET CUR_WHERE = '   AND  LRP.LrNS <> ''0000''' ; 
		END IF ; 
		
		IF P_ISOCCUPIED = 'Y' 
			AND TRIM ( UPPER ( P_NURSINGSTATION ) ) <> 'ALL' 
		THEN  -- Selected Nursing Station = 'N'                                       
			-- and Bed Type = 'All'                                                 
		SET CUR_WHERE = '                                                               
			AND  (TRIM(LRP.LrNS) = ''' || P_NURSINGSTATION || ''')' ; 
		END IF ; 
		
		IF ( P_ISOCCUPIED = 'N' ) 
			AND ( TRIM ( UPPER ( P_NURSINGSTATION ) ) <> 'ALL' ) 
		THEN SET CUR_WHERE = '                                                               
			AND TRIM(LRP.LrNS) = ''' || P_NURSINGSTATION || '''                                                                          
			AND LRP.LRACCT=0                ' ; 
		END IF ; 
		
		IF ( TRIM ( P_ISOCCUPIED ) = 'N' ) 
			AND ( TRIM ( UPPER ( P_NURSINGSTATION ) ) = 'ALL' ) 
		THEN SET CUR_WHERE = '  AND LRP.LRACCT=0 ' ; 
		END IF ; 
		
		SET INSQL = CUR_STR || CUR_WHERE || '                                                                  
		ORDER BY lrp.LRNS, lrp.LRROOM, lrp.LRBED ' ;  
		-- Add this because there is an error
		-- in the DB2 SQE !!!                                                   
		-- this will change where the QAQQINI                                   
		-- file is found for these queries                                      
--CALL QSYS/QCMDEXC ( 'CHGQRYA QRYOPTLIB(PACCESS)' , 0000000026.00000 ) ;       
	
		PREPARE S1 FROM INSQL ; 
		OPEN CURSOR1 ; 
		END P1  ;

