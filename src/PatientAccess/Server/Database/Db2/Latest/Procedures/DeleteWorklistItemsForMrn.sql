                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  DELETEWORKLISTITEMSFORMRN - SQL PROC FOR PX          */        
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
                                                                                
CREATE PROCEDURE DELETEWORKLISTITEMSFORMRN (                                    
        IN P_FACILITYID INTEGER ,                                               
        IN P_MEDICALRECORDNUMBER INTEGER )                                      
        LANGUAGE SQL                                                            
        SPECIFIC DLTWRKMRN                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE ACCNUM INTEGER DEFAULT 0 ;                              
                DECLARE RECORDEXISTS INTEGER DEFAULT 0 ;                        
                DECLARE ATEND INTEGER DEFAULT 0 ;                               
                DECLARE NOT_FOUND CONDITION FOR SQLSTATE '02000' ;              
                DECLARE ACCNUM_CURSOR CURSOR FOR                                
                SELECT AP.ACCOUNTNUMBER AS ACCOUNTNUMBER                        
                FROM ACCOUNTPROXIES AP                                          
                WHERE AP.MEDICALRECORDNUMBER = P_MEDICALRECORDNUMBER            
                AND AP.FACILITYID = P_FACILITYID;                               
                DECLARE CONTINUE HANDLER FOR NOT_FOUND                          
                SET ATEND = 1 ;                                                 
                                                                                
                OPEN ACCNUM_CURSOR ;                                            
                LOOP1 : LOOP                                                    
                FETCH ACCNUM_CURSOR INTO ACCNUM ;                               
                IF ATEND = 1 THEN                                               
                        LEAVE LOOP1 ;                                           
                        ELSE                                                    
                        CALL DELETEWORKLISTITEMS ( P_FACILITYID , ACCNUM ) ;    
                        END IF ;                                                
                        END LOOP ;                                              
                END P1  ;                                                       
