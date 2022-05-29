                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETBEDSTATUS - SQL PROC FOR PX                       */        
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
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE GETBEDSTATUS (                                                 
        IN P_NURSINGSTATION VARCHAR(2) ,                                        
        IN P_ROOM VARCHAR(5) ,                                                  
        IN P_BED VARCHAR(5) ,                                                   
        IN P_FACILITYID INTEGER ,                                               
        OUT O_PATIENTFNAME VARCHAR(50) ,                                        
        OUT O_PATIENTLNAME VARCHAR(50) ,                                        
        OUT O_PATIENTMI VARCHAR(3) ,                                            
        OUT O_ACCOUNTNUMBER INTEGER ,                                           
        OUT O_RESERVATIONRESULT VARCHAR(100) )                                  
        LANGUAGE SQL                                                            
        SPECIFIC GETBEDSTS                                                      
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
                DECLARE ACCCODE VARCHAR ( 10 ) ;                                
                                                                                
                SET O_PATIENTFNAME = NULL ;                                     
                SET O_PATIENTLNAME = NULL ;                                     
                SET O_PATIENTMI = NULL ;                                        
                SET O_ACCOUNTNUMBER = NULL ;                                    
                                                                                
                SELECT COUNT ( LRNS ) INTO NSCOUNT                              
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRNS = P_NURSINGSTATION ;                                   
                                                                                
                IF NSCOUNT > 0 THEN                                             
                SELECT COUNT ( LRROOM ) INTO ROOMCOUNT                          
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRROOM = P_ROOM                                             
                AND LRNS = P_NURSINGSTATION ;                                   
                                                                                
                IF ROOMCOUNT > 0 THEN                                           
                SELECT COUNT ( LRBED ) INTO BEDCOUNT                            
                FROM HPADLRP                                                    
                WHERE LRHSP# = P_FACILITYID                                     
                AND LRNS = P_NURSINGSTATION                                     
                AND LRROOM = P_ROOM                                             
                AND LRBED = P_BED ;                                             
                                                                                
                IF BEDCOUNT > 0 THEN                                            
                SELECT LRACCT ,                                                 
                LRFLG INTO ACCTNUMBER ,                                         
                PENDINGUPDATEFLAG                                               
                FROM HPADLRP                                                    
                WHERE LRNS = P_NURSINGSTATION                                   
                AND LRROOM = P_ROOM                                             
                AND LRBED = P_BED                                               
                AND LRHSP# = P_FACILITYID ;                                     
                                                                                
                IF PENDINGUPDATEFLAG = 'A' THEN                                 
                SET O_RESERVATIONRESULT = 'Reserved' ;                          
                                                                                
                ELSEIF TRIM ( PENDINGUPDATEFLAG ) = '' THEN                     
                IF ACCTNUMBER = 0 THEN                                          
                SET O_RESERVATIONRESULT = 'Available' ;                         
                ELSE                                                            
                SELECT AP.FIRSTNAME ,                                           
                AP.LASTNAME ,                                                   
                AP.MIDDLEINITIAL ,                                              
                AP.ACCOUNTNUMBER                                                
                INTO O_PATIENTFNAME ,                                           
                O_PATIENTLNAME ,                                                
                O_PATIENTMI ,                                                   
                O_ACCOUNTNUMBER                                                 
                FROM ACCOUNTPROXIES AP                                          
                WHERE FACILITYID = P_FACILITYID                                 
                AND ACCOUNTNUMBER = ACCTNUMBER                                  
                AND NURSINGSTATION = P_NURSINGSTATION                           
                AND ROOM = P_ROOM                                               
                AND BED = P_BED ;                                               
                                                                                
                SET     O_RESERVATIONRESULT = 'Occupied' ;                      
                END IF ;                                                        
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
