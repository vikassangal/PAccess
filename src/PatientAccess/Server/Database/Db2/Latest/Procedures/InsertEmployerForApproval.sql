                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  INSERTEMPLOYERFORAPPROVAL - SQL PROC FOR PX          */        
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
/* 11/18/2008 I  Sanjeev Kumar     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE INSERTEMPLOYERFORAPPROVAL (                                            
        IN P_EMFUUN VARCHAR(3) ,                                                
        IN P_EMNAME VARCHAR(25) ,                                               
        IN P_EMURFG VARCHAR(1) ,                                                
        IN P_EMADDT INTEGER ,                                                   
        IN P_EMLMDT INTEGER ,                                                   
        IN P_EMDLDT INTEGER ,                                                   
        IN P_EMCODE INTEGER ,                                                   
        IN P_EMACNT INTEGER ,                                                   
        IN P_EMUSER VARCHAR(255) ,                                               
        IN P_EMNEID VARCHAR(10) , 
        OUT IVAR INTEGER
        )   
        LANGUAGE SQL                                                            
        SPECIFIC INEMPFORAP
        DETERMINISTIC    
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        P1 : BEGIN                                            
                                                                                
                        INSERT INTO EMPLOYERSFORAPPROVAL VALUES ( 
                        DEFAULT ,                           
                        DEFAULT ,                           
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
                        
			VALUES IDENTITY_VAL_LOCAL() INTO IVAR ;
                                              
        END P1  ;                                               
 