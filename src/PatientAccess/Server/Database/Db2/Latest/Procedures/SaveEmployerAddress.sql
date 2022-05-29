                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SAVEEMPLOYERADDRESS - SQL PROC FOR PX                */        
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
/* 09/07/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
                                                                                
CREATE PROCEDURE SAVEEMPLOYERADDRESS (                                          
                                                                                
        IN P_FOLLOWUP_UNIT_ID VARCHAR(3) ,                                      
                                                                                
        IN P_EMPLOYER_CODE INTEGER ,                                            
                                                                                
        IN P_EMPLOYER_ADDRESS VARCHAR(25) ,                                     
                                                                                
        IN P_EMPLOYER_CITY VARCHAR(17) ,                                        
                                                                                
        IN P_EMPLOYER_STATE VARCHAR(2) ,                                        
                                                                                
        IN P_EMPLOYER_ZIP VARCHAR(9) ,                                          
                                                                                
        IN P_EMPLOYER_PHONE VARCHAR(10) ,                                       
                                                                                
        IN P_ADD_DATE INTEGER ,                                                 
                                                                                
        IN P_LAST_MAINT_DATE INTEGER ,                                          
                                                                                
        IN P_EMPLOYER_NAME VARCHAR(25) )                                        
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC SAVEMPADR                                                      
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
                DECLARE P_EMPLOYER_ADDR_ID INTEGER ;                            
                                                                                
                                                                                
                                                                                
                SELECT MAX ( EMADD# ) + 1 INTO P_EMPLOYER_ADDR_ID               
                                                                                
                FROM NCEMADP                                                    
                                                                                
                WHERE EMCODE = P_EMPLOYER_CODE ;                                
                                                                                
                                                                                
                                                                                
                IF P_EMPLOYER_ADDR_ID IS NULL THEN                              
                                                                                
                        SET P_EMPLOYER_ADDR_ID = 0 ;                            
                                                                                
                END IF ;                                                        
                                                                                
                                                                                
                                                                                
                IF P_EMPLOYER_PHONE IS NULL OR P_EMPLOYER_PHONE = '' THEN       
                                                                                
                        INSERT INTO NCEMADP (                                   
                        EMFUUN ,                                                
                        EMCODE ,                                                
                        EMADD# ,                                                
                        EMADDR ,                                                
                        EMCITY ,                                                
                        EMST ,                                                  
                        EMZIP ,                                                 
                        EMADDT ,                                                
                        EMLMDT ,                                                
                        EMANAM )                                                
                                                                                
                        VALUES (                                                
                        P_FOLLOWUP_UNIT_ID ,                                    
                        P_EMPLOYER_CODE ,                                       
                        P_EMPLOYER_ADDR_ID ,                                    
                        P_EMPLOYER_ADDRESS ,                                    
                        P_EMPLOYER_CITY ,                                       
                        P_EMPLOYER_STATE ,                                      
                        P_EMPLOYER_ZIP ,                                        
                        P_ADD_DATE ,                                            
                        P_LAST_MAINT_DATE ,                                     
                        P_EMPLOYER_NAME ) ;                                     
                                                                                
                ELSE                                                            
                                                                                
                        INSERT INTO NCEMADP (                                   
                        EMFUUN ,                                                
                        EMCODE ,                                                
                        EMADD# ,                                                
                        EMADDR ,                                                
                        EMCITY ,                                                
                        EMST ,                                                  
                        EMZIP ,                                                 
                        EMPH# ,                                                 
                        EMADDT ,                                                
                        EMLMDT ,                                                
                        EMANAM )                                                
                                                                                
                        VALUES (                                                
                        P_FOLLOWUP_UNIT_ID ,                                    
                        P_EMPLOYER_CODE ,                                       
                        P_EMPLOYER_ADDR_ID ,                                    
                        P_EMPLOYER_ADDRESS ,                                    
                        P_EMPLOYER_CITY ,                                       
                        P_EMPLOYER_STATE ,                                      
                        P_EMPLOYER_ZIP ,                                        
                        DECIMAL ( P_EMPLOYER_PHONE , 10 , 0 ) ,                 
                        P_ADD_DATE ,                                            
                        P_LAST_MAINT_DATE ,                                     
                        P_EMPLOYER_NAME ) ;                                     
                                                                                
                END IF ;                                                        
                                                                                
                                                                                
                                                                                
                UPDATE NCEM10P                                                  
                                                                                
                SET EMACNT = P_EMPLOYER_ADDR_ID                                 
                                                                                
                WHERE EMFUUN = P_FOLLOWUP_UNIT_ID                               
                                                                                
                AND EMCODE = P_EMPLOYER_CODE ;                                  
                                                                                
        END P1  ;                                                               
