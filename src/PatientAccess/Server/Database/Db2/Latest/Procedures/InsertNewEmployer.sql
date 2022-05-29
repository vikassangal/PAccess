                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  INSERTNEWEMPLOYER - SQL PROC FOR PX                  */        
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
                                                                                
CREATE PROCEDURE INSERTNEWEMPLOYER (                                            
        IN P_EMFUUN VARCHAR(3) ,                                                
        IN P_EMNAME VARCHAR(25) ,                                               
        IN P_EMURFG VARCHAR(1) ,                                                
        IN P_EMADDT INTEGER ,                                                   
        IN P_EMLMDT INTEGER ,                                                   
        IN P_EMDLDT INTEGER ,                                                   
        IN P_EMCODE INTEGER ,                                                   
        IN P_EMACNT INTEGER ,                                                   
        IN P_EMUSER VARCHAR(10) ,                                               
        IN P_EMNEID VARCHAR(10) )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC INSNEWEMP                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                        INSERT INTO NCEM10P VALUES (                            
                        P_EMFUUN ,                                              
                        P_EMNAME ,                                              
                        P_EMURFG ,                                              
                        P_EMADDT ,                                              
                        P_EMLMDT ,                                              
                        P_EMDLDT ,                                              
                        P_EMCODE ,                                              
                        P_EMACNT ,                                              
                        P_EMUSER ,                                              
                        P_EMNEID ) ;                                            
                        END P1  ;                                               
