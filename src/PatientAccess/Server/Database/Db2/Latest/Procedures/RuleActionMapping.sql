                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  RULEACTIONMAPPING - SQL PROC FOR PX                  */        
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
                                                                                
CREATE PROCEDURE RULEACTIONMAPPING ( )                                          
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC RULEACTMAP                                                     
                                                                                
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        RA.RULEID ,                                                             
        A.ACTIONID ,                                                            
        A.TYPE AS TYPE ,                                                        
        A.DESCRIPTION ,                                                         
        CAA.COMPOSITEACTIONID ,                                                 
        ( SELECT CASE WHEN COUNT ( A2.COMPOSITEACTIONID ) > 0                   
        THEN 1 ELSE 0 END                                                       
        FROM COMPOSITEACTION A2                                                 
        WHERE A2.COMPOSITEACTIONID = A.ACTIONID                                 
        ) AS ISCOMPOSITE                                                        
        FROM ACTION A                                                           
        LEFT OUTER JOIN COMPOSITEACTIONACTION CAA                               
        ON CAA.ACTIONID = A.ACTIONID                                            
        JOIN RULEACTION RA ON A.ACTIONID = RA.ACTIONID                          
        ORDER BY RULEID ;                                                       
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
