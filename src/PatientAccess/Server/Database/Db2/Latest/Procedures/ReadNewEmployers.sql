                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  READNEWEMPLOYERS - SQL PROC FOR PX                   */        
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
                                                                                
CREATE PROCEDURE READNEWEMPLOYERS ( )                                           
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC READNEWEMP                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        EMFUUN ,                                                
                        EMNAME ,                                                
                        EMURFG ,                                                
                        EMADDT ,                                                
                        EMLMDT ,                                                
                        EMDLDT ,                                                
                        EMCODE ,                                                
                        EMACNT ,                                                
                        EMUSER ,                                                
                        EMNEID ,                                                
                        TXNDATE ,                                               
                        ACTION                                                  
                        FROM EMPTXN EMP                                         
                        WITH CS KEEP LOCKS ;                                    
                        OPEN CURSOR1 ;                                          
                        END P1  ;                                               
