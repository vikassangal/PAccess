																				
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCOUNTSMATCHINGPATIENT - SQL PROC FOR PX            */        
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
/*************I********************I***********************************/        
																				
  SET PATH *LIBL ;                                                              
																				
  CREATE PROCEDURE ACCOUNTSMATCHINGPATIENT (                                    
	  IN P_FIRSTNAME VARCHAR(30) ,                                              
	  IN P_LASTNAME VARCHAR(30) ,                                               
	  IN P_ACCOUNTNUMBER INTEGER ,                                              
	  IN P_FACILITYID INTEGER )                                                 
	  DYNAMIC RESULT SETS 1                                                     
	  LANGUAGE SQL                                                              
	  SPECIFIC ACCMCHPAT                                                        
	  NOT DETERMINISTIC                                                         
	  READS SQL DATA                                                            
	  CALLED ON NULL INPUT                                                      
	  SET OPTION DBGVIEW = *SOURCE                                              
	  P1 : BEGIN                                                                
		 DECLARE CURSOR1 CURSOR FOR                                             
		 SELECT  AV.FACILITYID ,                                                
		 AV.ACCOUNTNUMBER ,                                                     
		 AV.ADMISSIONDATE ,                                                     
		 AV.DISCHARGEDATE ,                                                     
		 AV.SERVICECODE ,                                                       
		 AV.CLINICCODEID ,                                                      
		 AV.CLINICCODE ,                                                        
		 AV.CLINICDESCRIPTION ,                                                 
		 AV.FINANCIALCODE ,                                                     
		 AV.FIRSTNAME ,                                                         
		 AV.LASTNAME ,                                                          
		 AV.MIDDLEINITIAL ,                                                     
		 AV.PATIENTTYPE ,                                                       
		 AV.OPSTATUSCODE ,                                                      
		 AV.IPSTATUSCODE ,                                                      
		 AV.FINALBILLINGFLAG ,                                                  
		 AV.OPVISITNUMBER ,                                                     
		 AV.PENDINGPURGE ,                                                      
		 AV.UNBILLEDBALANCE ,                                                   
		 AV.MEDICALSERVICECODE ,                                                
		 AV.LOCKINDICATOR ,                                                     
		 AV.LOCKERPBARID ,                                                      
		 AV.LOCKDATE ,                                                          
		 AV.LOCKTIME ,
		 AV.SHORTREGTYPE AS SHORTREGISTRATIONTYPE ,                             
		 AV.OPERATINGDRID ,                                                     
		 AV.ATTENDINGDRID ,                                                     
		 AV.ADMITTINGDRID ,                                                     
		 AV.REFERINGDRID ,                                                      
		 AV.OTHERDRID ,                                                         
		 AV.CONSULTINGDR1ID ,                                                   
		 AV.CONSULTINGDR2ID ,                                                   
		 AV.CONSULTINGDR3ID ,                                                   
		 AV.CONSULTINGDR4ID ,                                                   
		 AV.CONSULTINGDR5ID ,                                                   
		 AV.NURSINGSTATION ,                                                    
		 AV.ROOM ,                                                              
		 AV.BED ,                                                               
		 AV.ISOLATIONCODE ,                                                     
		 AV.PENDINGDISCHARGE ,                                                  
		 AV.DISCHARGECODE ,                                                     
		 AV.VALUABLESARETAKEN ,                                                 
		 AV.ABSTRACTEXISTS ,                                                    
		 AV.CHIEFCOMPLAINT ,                                                    
		 AV.MEDICALRECORDNUMBER ,                                               
		 AV.DOB ,                                                               
		 AV.GENDERID ,                                                          
		 AV.BLOODLESS ,                                                         
		 AV.ACCOMODATIONCODE ,  
		 AV.ACCOMODATIONDESC,                                                
		 AV.OPTOUT AS OPTOUT ,                                                  
		 AV.CONFIDENTIAL ,                                                      
		 AV.LASTMAINTENANCEDATE ,                                               
		 AV.LASTMAINTENANCELOGNUMBER ,                                          
		 AV.UPDATELOGNUMBER                                                     
		 FROM ACCOUNTPROXIES AV                                                 
		 WHERE AV.FACILITYID = P_FACILITYID                                     
		 AND ( ( AV.ACCOUNTNUMBER IS NULL OR                                    
		 AV.ACCOUNTNUMBER = P_ACCOUNTNUMBER ) OR                                
		 ( ( ( P_FIRSTNAME <> '' ) AND ( P_LASTNAME = '' ) AND                  
		 ( UPPER ( AV . FIRSTNAME )                                             
		 LIKE CONCAT ( UPPER ( P_FIRSTNAME ) , '%' ) ) ) OR                     
		 ( ( P_LASTNAME <> '' ) AND ( P_FIRSTNAME = '' ) AND                    
		 ( UPPER ( AV . LASTNAME )                                              
		 LIKE CONCAT ( UPPER ( P_LASTNAME ) , '%' ) ) ) OR                      
		 ( ( P_FIRSTNAME <> '' ) AND ( P_LASTNAME <> '' ) AND                   
		 ( ( UPPER ( AV.FIRSTNAME )                                             
		 LIKE CONCAT ( UPPER ( P_FIRSTNAME ) , '%' ) ) AND                      
		 ( UPPER ( AV.LASTNAME ) LIKE CONCAT                                    
		 ( UPPER ( P_LASTNAME ) , '%' ) ) ) ) ) )                               
		 AND ( ( AV.DISCHARGEDATE IS NULL ) OR                                  
		 DATE ( AV.DISCHARGEDATE ) = CURRENT DATE OR                            
		 ( AV.PENDINGDISCHARGE = 'Y' ) )                                        
		 AND ( AV.PATIENTTYPE IN ( '1' , '2' , '3' , '9' )                      
		 OR ( AV.PATIENTTYPE = '4'                                              
		 AND ( DATE ( AV.ADMISSIONDATE ) = CURRENT DATE                         
		 OR AV.BED <> '' ) ) )                                                  
		 ORDER BY AV.LASTNAME , AV.FIRSTNAME , AV.MIDDLEINITIAL ;               
		 OPEN CURSOR1 ;                                                         
		 END P1  ;                                                              
