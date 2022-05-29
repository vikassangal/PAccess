                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTSPECIFICIPACLINIC - SQL PROC FOR PX            */        
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
                                                                                
CREATE PROCEDURE SELECTSPECIFICIPACLINIC (                                      
        IN @NHHSPC VARCHAR(3) ,                                                 
        IN @IPACODE VARCHAR(5) ,                                                
        IN @CLINICCODE VARCHAR(2) )                                             
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELSPECIPA                                                     
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                        DECLARE CURSOR1 CURSOR FOR              
                                        SELECT                                  
                                        NHIPA ,                                 
                                        NHIPAC ,                                
                                        NHIPAN ,                                
                                        NHIPCN                                  
                                        FROM NH0301P                            
                                        WHERE NHIPA = @IPACODE                  
                                        AND NHIPAC = @CLINICCODE                
                                        AND NHHSPC = @NHHSPC ;                  
                                        OPEN CURSOR1 ;                          
                                        END P1  ;                               
