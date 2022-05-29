                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ALLRULES - SQL PROC FOR PATIENT ACCESS               */        
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
                                                                                
CREATE PROCEDURE ALLRULES ( )                                                   
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ALLRULE                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT DISTINCT                                                         
        (                                                                       
        SELECT CASE WHEN COUNT                                                  
                ( R2.COMPOSITERULEID ) > 0                                      
        THEN 1 ELSE 0 END                                                       
        FROM COMPOSITERULE R2                                                   
        WHERE R2.COMPOSITERULEID = R.RULEID                                     
        ) AS ISCOMPOSITE ,                                                      
        R.RULEID ,                                                              
        R.TYPE ,                                                                
        R.DESCRIPTION ,                                                         
        R.SEVERITY ,                                                            
        CR.COMPOSITERULEID ,                                                    
        CR.TYPE AS COMPOSITERULETYPE                                            
        FROM RULE R                                                             
        LEFT OUTER JOIN COMPOSITERULERULE CRR                                   
        ON R.RULEID = CRR.RULEID                                                
        LEFT OUTER JOIN COMPOSITERULE CR                                        
        ON CRR.COMPOSITERULEID =                                                
                CR.COMPOSITERULEID                                              
        ORDER BY R.TYPE ;                                                       
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
