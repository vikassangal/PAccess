                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  LOCATIONMATCHINGWITHGENDER - SQL PROC FOR PX         */        
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
/* 02/26/2008 I Kevin sedota       I removed use of proxies and       */
/*                                  stopped using CQE                 */
/* 03/10/2008 I kjs                I fixed query to not default to    */
/*                                    hospital #900                   */
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                       

CREATE PROCEDURE LOCATIONMATCHINGWITHGENDER (                                                                                                               
        IN P_FACILITYID INTEGER ,                                              
        IN P_ISOCCUPIED VARCHAR(1) ,                                             
        IN P_GENDER VARCHAR(1) ,                                                
        IN P_NURSINGSTATION VARCHAR(5) ,                                        
        IN P_ROOM VARCHAR(5) )                                                  
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                             
        SPECIFIC LOCMCHWG                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                            
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
        DECLARE SEX1 CHAR(1);
        DECLARE SEX2 CHAR(1);                                                                        
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
            -- look for and exclude rooms that are not acceptable because they
            -- contain a patient of the opposite sex. The genders are set below
            -- BE VERY CAREFUL - All may not be as it seems. There are 3 possible sexes. 
            -- and we need to exclude rooms that contain patients of sexes other than
            -- the patient. We can not use 'positive' logic and say find all rooms where 
            -- the sex of the current patient matches because this will exclude empty
            -- rooms. Therefore we use this clause to exclude rooms that hold a patient 
            -- of a sex other than the one we are looking for.
            AND  ( LRP . LRNS || LRP . LRROOM  NOT IN  
                (SELECT LPNS || LPROOM 
	FROM HPADLPP
	join HPADMDP on (MDHSP# = P_FACILITYID and MDMRC# = LPMRC#)
                WHERE LRHSP# = P_FACILITYID 
                AND LPLDD = 0
                AND (MDSEX = SEX1 OR MDSEX = SEX2)
	)  )                       
            AND                                                             
            (                                                               
                ( P_ISOCCUPIED = 'Y'                                            
                AND P_NURSINGSTATION IS NULL                                    
                AND P_ROOM IS NULL )                                            
                                                                                
                OR                                                              
                                                                                
                ( P_ISOCCUPIED = 'Y'                                            
                AND P_NURSINGSTATION IS NOT NULL                                
                AND P_ROOM IS NULL                                              
                AND trim(LRP . LRNS) = trim(P_NURSINGSTATION) )                             
                                                                                
                OR                                                              
                                                                                
                ( P_ISOCCUPIED = 'N'                                            
                AND P_NURSINGSTATION IS NULL                                    
                AND P_ROOM IS NULL                                              
                AND ( LRP . LRNS || LRP . LRROOM IN                             
                ( SELECT DISTINCT LRNS || LRROOM                                
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRACCT = 0                                                  
                AND LRFLG = '' ) ) )                                            
                                                                                
                OR                                                              
                                                                                
                ( P_ISOCCUPIED = 'N'                                            
                AND P_NURSINGSTATION IS NOT NULL                                
                AND P_ROOM IS NULL                                              

                AND ( LRP . LRNS || LRP . LRROOM IN                             

                ( SELECT DISTINCT LRNS || LRROOM                                
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRACCT = 0                                                  
                AND LRFLG = '' ) )                                              
                AND ( trim(LRP . LRNS) = trim(P_NURSINGSTATION) )                           
                                                                                
                OR                                                              
                                                                                
                ( P_ISOCCUPIED = 'N'                                            
                AND P_NURSINGSTATION IS NOT NULL                                
                AND P_ROOM IS NOT NULL                                          
                AND ( LRP . LRNS || LRP . LRROOM IN                             
                ( SELECT DISTINCT LRNS || LRROOM                                
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRACCT = 0                                                  
                AND LRFLG = '' ) )                                              
                AND ( trim(LRP . LRNS) = trim(P_NURSINGSTATION)                             
                AND trim(LRP . LRROOM) = trim(P_ROOM) )                                     
                )                                                               
              )                                                       
            )                                                       
                                                                            
            ORDER BY                                                        
            LRP . LRNS ,                                                    
            LRP . LRROOM ,                                                  
            LRP . LRBED ;                                                   
                                                                           
            IF P_NURSINGSTATION = 'ALL' THEN                                
            SET P_NURSINGSTATION = NULL ;                                   
            END IF ;                                                        
                                                                            
            IF P_ROOM = 'ALL' THEN                                          
            SET P_ROOM = NULL ;                                             
            END IF ;     

            -- we need to flip the gender
            -- if the paremter is M then we want rooms that do not contain a Female.
            -- and be careful. You can not use <> 'M' because this will ignore blanks
            -- which we need.

            IF P_GENDER = 'M'   THEN
				SET SEX1 = 'F';
				SET SEX2 = 'U';
			ELSEIF P_GENDER = 'F' THEN
				SET SEX1 = 'M' ;
				SET SEX2 = 'U';
			ELSE -- P_GENDER = 'U'
				SET SEX1 = 'M';
				SET SEX2 = 'F';
			END IF;
            --CALL QSYS / QCMDEXC ( 'CHGQRYA QRYOPTLIB(PACCESS)' , 0000000026.00000 ) ;
            OPEN CURSOR1 ;                                                  
END P1  ;                                                       
