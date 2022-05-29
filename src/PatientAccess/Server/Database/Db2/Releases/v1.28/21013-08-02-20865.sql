﻿																				
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTACCOUNTPROXYFOR - SQL PROC FOR PX              */        
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
/* 07/21/2011 I  Nereid Tavares    I SR1557 - Read new column to      */        
/*            I                    I   denote Newborn Reg acct        */   
/* 05/29/2013 I  Srilakshmi        I select Isolation Code            */    
/*************I********************I***********************************/        
																				
SET PATH *LIBL ;                                                                
																				
CREATE PROCEDURE SELECTACCOUNTPROXYFOR (                                        
		IN P_HSP INTEGER ,                                                      
		IN P_MRC INTEGER ,                                                      
		IN P_ACCOUNTNUMBER INTEGER )                                            
		DYNAMIC RESULT SETS 1                                                   
		LANGUAGE SQL                                                            
		SPECIFIC SELACCPXY                                                      
		NOT DETERMINISTIC                                                       
		MODIFIES SQL DATA                                                       
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
		
			DECLARE CURSOR1 CURSOR FOR 
			SELECT 
			V . ACCOUNTNUMBER AS ACCOUNTNUMBER , 
			V . ADMISSIONDATE AS ADMISSIONDATE , 
			V . DISCHARGEDATE AS DISCHARGEDATE , 
			V . SERVICECODE AS SERVICECODE , 
			V . CLINICCODEID AS CLINICCODEID , 
			V . CLINICCODE AS CLINICCODE , 
			V . CLINICDESCRIPTION AS CLINICDESCRIPTION , 
			V . FINANCIALCODE AS FINANCIALCODE , 
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
			V . ACCOMODATIONCODE , 
			V . ACCOMODATIONDESC , 
			V . VALUABLESARETAKEN AS VALUABLESARETAKEN , 
			V . PENDINGDISCHARGE AS PENDINGDISCHARGE , 
			V . ABSTRACTEXISTS AS ABSTRACTEXISTS , 
			V . DISCHARGECODE AS DISCHARGECODE , 
			V . SHORTREGTYPE AS SHORTREGISTRATIONTYPE ,   
			V . NEWBORNREGTYPE AS NEWBORNREGISTRATIONTYPE ,   
			V . OPERATINGDRID , 
			V . ATTENDINGDRID , 
			V . ADMITTINGDRID , 
			V . REFERINGDRID , 
			V . OTHERDRID , 
			V . CONSULTINGDR1ID , 
			V . CONSULTINGDR2ID , 
			V . CONSULTINGDR3ID , 
			V . CONSULTINGDR4ID , 
			V . CONSULTINGDR5ID , 
			V . ADMITSOURCECODE , 
			E . HBINS# AS PRIMARYPLANID , 
			V . LOCKINDICATOR , 
			V . LOCKERPBARID , 
			V . LOCKDATE , 
			V . LOCKTIME , 
			V . LASTMAINTENANCEDATE AS LASTMAINTENANCEDATE , 
			V . LASTMAINTENANCELOGNUMBER AS LASTMAINTENANCELOGNUMBER , 
			V . ISOLATIONCODE,
			V . UPDATELOGNUMBER AS UPDATELOGNUMBER 
			
			FROM ACCOUNTPROXIES V 
			LEFT JOIN HPADHBP E ON 
				( V . FACILITYID = E . HBHSP# 
				AND V . MEDICALRECORDNUMBER = E . HBMRC# 
				AND V . ACCOUNTNUMBER = E . HBACCT 
				AND E . HBSEQ# = 1 ) 
			
			WHERE V . FACILITYID = P_HSP 
				AND V . MEDICALRECORDNUMBER = P_MRC 
				AND V . ACCOUNTNUMBER = P_ACCOUNTNUMBER ; 
			
			OPEN CURSOR1 ; 
			END P1  ;

