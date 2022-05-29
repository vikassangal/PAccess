                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  LOCATIONMATCHINGWOGENDER - SQL PROC FOR PX           */        
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
/* 08/10/2006 I  KEVIN SEDOTA      I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
  
CREATE PROCEDURE LOCATIONMATCHINGWOGENDER ( 
	IN P_FACILITYID INTEGER , 
	IN P_ISOCCUPIED VARCHAR(1) , 
	IN P_GENDER VARCHAR(1) , 
	IN P_NURSINGSTATION VARCHAR(5) , 
	IN P_ROOM VARCHAR(5) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC LOCMCHWOG 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	
	P1 : BEGIN 
				 
	DECLARE CURSOR1 CURSOR FOR 
	 
	SELECT 
	LRP . LRNS AS NURSINGSTATION ,                                          
        LRP . LRROOM AS ROOM ,                                                  
        LRP . LRBED AS BED ,                                                    
        LRP . LROV AS OVERFLOWFLAG ,                                            
        LRP . LRRCON AS ROOMCONDITION ,                                         
        AV . LPHSP# as FACILITYID ,                                                       
        AV . LPACCT AS ACCOUNTNUMBER ,                                                    
        AV . LPPFNM as FIRSTNAME ,                                                        
        AV . LPPLNM as LASTNAME ,                                                         
        AV . LPPMI as MIDDLEINITIAL ,                                                    
        AV . LPPTYP as PATIENTTYPE ,                                                      
        AV . LPADR# as ATTENDINGDRID ,                                                    
        AV . LPISO AS ISOLATIONCODE ,                                                    
        AV . LPDSFL as PENDINGDISCHARGE ,                                                 
        AV . LPDCOD as DISCHARGECODE ,                                                    
        AV . LPVALU as VALUABLESARETAKEN ,                                                
        AV . LPCLVS as CLERGYVISIT ,                                                      
        AV . LPMRC# as MEDICALRECORDNUMBER ,                                              
        M . MDDOB as DOB ,                                                              
        M . MDSEX AS GENDERID ,                                                         
        LRP . LRAVFG AS UNOCCUPIED ,                                            
        LRP . LRFLG AS PENDINGADMISSION 
	 
	FROM HPADLRP LRP                                                        
            LEFT OUTER JOIN HPADLPP AV ON                            
                ( LRP . LRACCT = AV . LPACCT  AND AV . LPHSP# = P_FACILITYID  
                AND TRIM ( AV . LPNS ) <> '' )                        
            LEFT OUTER JOIN HPADMDP m on
			(MDHSP# = P_FACILITYID and MDMRC# = AV.LPMRC#)                     
            WHERE LRP . LRHSP# = P_FACILITYID                               
            AND LRP . LRFLG <> 'D' 
	
	AND 
		( ( P_ISOCCUPIED = 'Y' 
		AND P_NURSINGSTATION IS NULL 
		AND P_ROOM IS NULL ) 
		OR 
		( P_ISOCCUPIED = 'Y' 
		AND P_NURSINGSTATION IS NOT NULL 
		AND P_ROOM IS NULL 
		AND TRIM ( LRP . LRNS ) = TRIM ( P_NURSINGSTATION ) ) 
	OR 
		( P_ISOCCUPIED = 'Y' 
		AND P_NURSINGSTATION IS NOT NULL 
		AND P_ROOM IS NOT NULL 
		AND TRIM ( LRP . LRNS ) = TRIM ( P_NURSINGSTATION ) 
		AND LRP . LRROOM = P_ROOM ) 
	OR 
		( P_ISOCCUPIED = 'N' 
		AND P_NURSINGSTATION IS NULL 
		AND P_ROOM IS NULL 
		AND ( LRP . LRNS || LRP . LRROOM IN 
		( SELECT DISTINCT LRNS || LRROOM 
			FROM HPADLRP 
			WHERE LRHSP# = P_FACILITYID 
			AND LRACCT = 0 AND LRFLG = '' ) 
		OR AV . LPNS = '' ) ) 
	OR 
		( P_ISOCCUPIED = 'N' 
		AND P_NURSINGSTATION IS NOT NULL 
		AND P_ROOM IS NULL 
		AND ( LRP . LRNS || LRP . LRROOM IN 
		( SELECT DISTINCT LRNS || LRROOM 
			FROM HPADLRP 
			WHERE LRHSP# = P_FACILITYID 
			AND LRACCT = 0 AND LRFLG = '' ) 
		OR AV . LPNS = '' ) 
		AND ( TRIM ( LRP . LRNS ) = TRIM ( P_NURSINGSTATION ) ) ) 
	OR 
		( P_ISOCCUPIED = 'N' 
		AND P_NURSINGSTATION IS NOT NULL 
		AND P_ROOM IS NOT NULL 
		AND ( LRP . LRNS || LRP . LRROOM IN 
		( SELECT DISTINCT LRNS || LRROOM 
			FROM HPADLRP WHERE LRHSP# = P_FACILITYID 
			AND LRACCT = 0 AND LRFLG = '' ) 
		OR AV . LPNS = '' ) 
		AND ( TRIM ( LRP . LRNS ) = TRIM ( P_NURSINGSTATION ) 
		AND LRP . LRROOM = P_ROOM ) ) ) 
	 
	ORDER BY 
	LRP . LRNS , 
	LRP . LRROOM , 
	LRP . LRBED ; 
	 
	IF P_NURSINGSTATION = 'ALL' 
	THEN SET P_NURSINGSTATION = NULL ; 
	 
	END IF ; 
	 
	IF P_ROOM = 'ALL' 
	THEN SET P_ROOM = NULL ; 
	 
	END IF ; 
	--CALL QSYS / QCMDEXC ( 'CHGQRYA QRYOPTLIB(PACCESS)' , 0000000026.00000 ) ; 
	 
	OPEN CURSOR1 ; 
	 
	END P1  ;
