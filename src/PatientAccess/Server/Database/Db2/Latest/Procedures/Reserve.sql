                                                                                
/**********************************************************************/        
/* AS400 Short Name: RESERVE                                          */        
/* iSeries400        RESERVE -SQL PROC FOR PATIENT ACCESS             */        
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
CREATE PROCEDURE RESERVE (                                                      
        IN P_OLD_NURSINGSTATION VARCHAR(2) ,                                    
        IN P_OLD_ROOM VARCHAR(5) ,                                              
        IN P_OLD_BED VARCHAR(5) ,                                               
        IN P_NEW_NURSINGSTATION VARCHAR(2) ,                                    
        IN P_NEW_ROOM VARCHAR(5) ,                                              
        IN P_NEW_BED VARCHAR(5) ,                                               
        IN P_FACILITYID INTEGER ,                                               
        IN P_PATIENTTYPECODE VARCHAR(2) ,                                       
        OUT O_PATIENTFNAME VARCHAR(50) ,                                        
        OUT O_PATIENTLNAME VARCHAR(50) ,                                        
        OUT O_PATIENTMI VARCHAR(3) ,                                            
        OUT O_ACCOUNTNUMBER INTEGER ,                                           
        OUT O_RESERVATIONRESULT VARCHAR(100) )                                  
        LANGUAGE SQL                                                            
        SPECIFIC RESERVE                                                        
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        BEGIN                                                                   
                                                                                
                                                                                
        DECLARE PENDINGUPDATEFLAG       VARCHAR ( 10 ) ;                        
                                                                                
        DECLARE ACCTNUMBER      INTEGER DEFAULT 0 ;                             
                                                                                
        DECLARE NSCOUNT INTEGER DEFAULT 0 ;                                     
                                                                                
        DECLARE ROOMCOUNT       INTEGER DEFAULT 0 ;                             
                                                                                
        DECLARE BEDCOUNT        INTEGER DEFAULT 0 ;                             
                                                                                
        DECLARE ACCCODE INTEGER DEFAULT 0 ;                                     
                                                                                
                                                                                
        SET O_PATIENTFNAME = NULL ;                                             
                                                                                
        SET O_PATIENTLNAME = NULL ;                                             
                                                                                
        SET O_PATIENTMI = NULL ;                                                
                                                                                
        SET O_ACCOUNTNUMBER = NULL ;                                            
                                                                                
                                                                                
        SELECT COUNT ( LRNS ) INTO NSCOUNT FROM HPADLRP                         
        WHERE LRHSP# = P_FACILITYID                                             
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) ;                                       
                                                                                
                                                                                
                                                                                
        IF NSCOUNT > 0 THEN                                                     
                                                                                
        SELECT COUNT ( LRROOM ) INTO ROOMCOUNT FROM HPADLRP                     
        WHERE LRHSP# = P_FACILITYID                                             
        AND LRROOM = P_NEW_ROOM AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) ;               
                                                                                
                                                                                
        IF ROOMCOUNT > 0 THEN                                                   
        SELECT COUNT ( LRBED ) INTO BEDCOUNT FROM HPADLRP                       
        WHERE LRHSP# = P_FACILITYID                                             
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) AND LRROOM = P_NEW_ROOM                 
        AND LRBED = P_NEW_BED ;                                                 
                                                                                
        IF BEDCOUNT > 0 THEN                                                    
                                                                                
        SELECT LRACCT , LRFLG INTO ACCTNUMBER ,                                 
        PENDINGUPDATEFLAG FROM HPADLRP                                          
        WHERE trim(LRNS) = trim(P_NEW_NURSINGSTATION) AND LRROOM = P_NEW_ROOM               
        AND LRBED = P_NEW_BED AND LRHSP# = P_FACILITYID ;                       
                                                                                
        IF PENDINGUPDATEFLAG = 'A' THEN                                         
                                                                                
        SET O_RESERVATIONRESULT = 'Reserved' ;                                  
                                                                                
        ELSEIF ACCTNUMBER = 0 AND TRIM ( PENDINGUPDATEFLAG ) = '' THEN          
                                                                                
        IF P_OLD_NURSINGSTATION IS NOT NULL AND P_OLD_ROOM IS NOT NULL          
        AND P_OLD_BED IS NOT NULL THEN                                          
                                                                                
        UPDATE HPADLRP SET LRFLG = '' WHERE LRHSP# = P_FACILITYID               
        AND trim(LRNS) = trim(P_OLD_NURSINGSTATION)
        AND LRROOM = P_OLD_ROOM AND LRBED = P_OLD_BED ;                         
                                                                                
        END IF ;                                                                
                                                                                
        SELECT COUNT ( MCITM# ) INTO ACCCODE FROM HPOCMCP                       
        WHERE MCHSP# = P_FACILITYID AND MCDEPT =                                
        ( SELECT LNDEPT FROM HPADLNP WHERE LNHSP# =                             
        P_FACILITYID AND trim(LNNS) = trim(P_NEW_NURSINGSTATION) ) ;                        
                                                                                
        IF ( P_PATIENTTYPECODE = '1' ) THEN                                     
                                                                                
        IF ACCCODE <> 0 THEN                                                    
                                                                                
        UPDATE HPADLRP SET LRFLG = 'A' WHERE LRHSP# = P_FACILITYID              
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) AND LRROOM = P_NEW_ROOM                 
        AND LRBED = P_NEW_BED ;                                                 
                                                                                
        SET O_RESERVATIONRESULT = 'Success' ;                                   
                                                                                
        ELSE                                                                    
                                                                                
        SET O_RESERVATIONRESULT = 'Bed not available for inpatient' ;           
                                                                                
        END IF ;                                                                
                                                                                
        END IF ;                                                                
                                                                                
        UPDATE HPADLRP SET LRFLG = 'A' WHERE LRHSP# = P_FACILITYID              
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION)
        AND LRROOM = P_NEW_ROOM AND LRBED = P_NEW_BED ;                         
                                                                                
        SET O_RESERVATIONRESULT = 'Success' ;                                   
                                                                                
        ELSE                                                                    
                                                                                
        SELECT LRACCT INTO ACCTNUMBER FROM HPADLRP WHERE                        
        LRHSP# = P_FACILITYID                                                   
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) AND LRROOM = P_NEW_ROOM                 
        AND LRBED = P_NEW_BED ;                                                 
                                                                                
        SELECT AP . FIRSTNAME , AP . LASTNAME ,                                 
        AP . MIDDLEINITIAL , AP . ACCOUNTNUMBER                                 
        INTO O_PATIENTFNAME , O_PATIENTLNAME , O_PATIENTMI ,                    
        O_ACCOUNTNUMBER                                                         
        FROM ACCOUNTPROXIES AP                                                  
        WHERE FACILITYID = P_FACILITYID AND ACCOUNTNUMBER = ACCTNUMBER          
        AND trim(NURSINGSTATION) = trim(P_NEW_NURSINGSTATION)                               
        AND ROOM = P_NEW_ROOM AND BED = P_NEW_BED ;                             
                                                                                
        SET     O_RESERVATIONRESULT = 'Occupied' ;                              
                                                                                
        END IF ;                                                                
                                                                                
        ELSE                                                                    
                                                                                
        SET O_RESERVATIONRESULT = 'Invalid Bed' ;                               
                                                                                
        END IF ;                                                                
                                                                                
        ELSE                                                                    
                                                                                
        SET O_RESERVATIONRESULT = 'Invalid Room' ;                              
                                                                                
        END IF ;                                                                
                                                                                
        ELSE                                                                    
                                                                                
        SET O_RESERVATIONRESULT = 'Invalid Nursingstation' ;                    
                                                                                
        END IF ;                                                                
                                                                                
        END  ;                                                                  
