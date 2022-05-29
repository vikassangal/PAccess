                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  UPDATENOSHOWWORKLIST - SQL PROC FOR PX               */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE UPDATENOSHOWWORKLIST (                                         
        IN P_HSP INTEGER )                                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC UPDNSW                                                         
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE ACCNUM INTEGER DEFAULT 0 ;                                      
        DECLARE RECORDEXISTS INTEGER DEFAULT 0 ;                                
        DECLARE ATEND INTEGER DEFAULT 0 ;                                       
        DECLARE NOT_FOUND CONDITION FOR SQLSTATE '02000' ;                      
        DECLARE NOSHOW_CURSOR CURSOR FOR                                        
                                                                                
        SELECT AP . ACCOUNTNUMBER                                               
        AS ACCOUNTNUMBER                                                        
        FROM ACCOUNTPROXIES AP WHERE                                            
        AP . ADMISSIONDATE < CURRENT TIMESTAMP AND                              
        AP . FACILITYID = P_HSP AND AP . PATIENTTYPE = '0' ;                    
        DECLARE CONTINUE HANDLER FOR    NOT_FOUND                               
        SET ATEND = 1 ;                                                         
        OPEN NOSHOW_CURSOR ;                                                    
        LOOP1 : LOOP                                                            
        FETCH NOSHOW_CURSOR INTO ACCNUM ;                                       
        IF ATEND = 1 THEN                                                       
        LEAVE LOOP1 ;                                                           
        ELSE                                                                    
        SELECT COUNT ( * ) INTO RECORDEXISTS FROM                               
        ACCOUNTWORKLISTITEMS WLI WHERE                                          
        WLI . ACCOUNTNUMBER = ACCNUM AND WLI . FACILITYID = P_HSP ;             
                                                                                
        IF RECORDEXISTS = 0 THEN                                                
        INSERT INTO ACCOUNTWORKLISTITEMS ( FACILITYID ,                         
        ACCOUNTNUMBER , WORKLISTID , COMPOSITERULEID , RULEID )                 
        VALUES (P_HSP , ACCNUM , 6 , 0 , 24) ;                                  
        END IF ;                                                                
        END IF ;                                                                
        END LOOP ;                                                              
        CLOSE NOSHOW_CURSOR ;                                                   
                                                                                
                                                                                
        END P1  ;                                                               
