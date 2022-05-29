                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTPATIENTSFORNAME - SQL PROC FOR PX              */        
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
                                                                                
CREATE PROCEDURE SELECTPATIENTSFORNAME (                                        
        IN @LPLNAME VARCHAR(32) ,                                               
        IN @LPHSP INTEGER )                                                     
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPAT4NAM                                                     
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        LPPLNM ,                                                
                        LPPFNM ,                                                
                        LPMRC# AS MRC                                           
                        FROM HPADLPU                                            
                        WHERE LPPLNM LIKE CONCAT ( @LPLNAME , '%' )             
                        AND LPHSP# = @LPHSP ;                                   
                        DECLARE CURSOR2 CURSOR FOR                              
                        SELECT                                                  
                        LPADT1 ,                                                
                        LPADT2 ,                                                
                        LPADT3 ,                                                
                        LPLAT                                                   
                        FROM HPADLPU                                            
                        WHERE LPPLNM LIKE CONCAT ( @LPLNAME , '%' )             
                        AND LPHSP# = @LPHSP ;                                   
                        OPEN CURSOR1 ;                                          
                        OPEN CURSOR2 ;                                          
                        END P1  ;                                               
