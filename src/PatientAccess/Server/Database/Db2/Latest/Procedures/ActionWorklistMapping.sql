                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACTIONWORKLISTMAPPING - SQL PROC FOR PX              */        
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
                                                                                
CREATE PROCEDURE ACTIONWORKLISTMAPPING ( )                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ACTWRKMAP                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
                SELECT RA.ACTIONID ,                                            
                        W.WORKLISTID ,                                          
                        WORKLISTNAME                                            
                        FROM RULEACTION RA                                      
                        JOIN WORKLISTRULES WLR                                  
                        ON RA.RULEID = WLR.RULEID                               
                        JOIN WORKLISTS W                                        
                        ON WLR.WORKLISTID = W.WORKLISTID                        
                        ORDER BY ACTIONID ;                                     
                         OPEN CURSOR1 ;                                         
                END P1  ;                                                       
