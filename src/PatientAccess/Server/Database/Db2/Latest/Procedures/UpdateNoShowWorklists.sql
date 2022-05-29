                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  UPDATENOSHOWWORKLISTS - SQL PROC FOR PX               */       
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
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE UPDATENOSHOWWORKLISTS (                                        
        IN P_HSP INTEGER )                                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC UPDNSWS                                                        
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                DECLARE RECORDEXISTS INTEGER DEFAULT 0 ;                        
                DECLARE ATEND INTEGER DEFAULT 0 ;                               
                DECLARE NOT_FOUND CONDITION FOR SQLSTATE '02000' ;              
                DECLARE HSPNUMBER INTEGER ;                                     
                DECLARE FACILITYCURSOR CURSOR FOR                               
                                                                                
                SELECT                                                          
                FACILITYID                                                      
                FROM FACILITIES ;                                                
                                                                                
                DECLARE CONTINUE HANDLER FOR NOT_FOUND                          
                                                                                
                SET ATEND = 1 ;                                                 
                                                                                
                OPEN FACILITYCURSOR ;                                           
                                                                                
                LOOP1 : LOOP                                                    
                                                                                
                FETCH FACILITYCURSOR INTO HSPNUMBER ;                           
                IF ATEND = 1 THEN                                               
                LEAVE LOOP1 ;                                                   
                ELSE                                                            
                                                                                
                CALL UPDATENOSHOWWORKLIST ( HSPNUMBER ) ;                       
                END IF ;                                                        
                                                                                
                END LOOP ;                                                      
                CLOSE FACILITYCURSOR ;                                          
                                                                                
                END P1  ;                                                       
