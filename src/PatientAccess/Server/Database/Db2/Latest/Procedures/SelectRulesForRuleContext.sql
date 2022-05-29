                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTRULESFORRULECONTEXT - SQL PROC FOR PX          */        
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
                                                                                
CREATE PROCEDURE SELECTRULESFORRULECONTEXT (                                    
        IN P_RULECONTEXT VARCHAR(150) )                                         
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELRULCXT                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                                                                                
                SELECT                                                          
                (                                                               
                SELECT CASE WHEN COUNT ( R2.COMPOSITERULEID ) > 0               
                THEN 1 ELSE 0 END                                               
                FROM COMPOSITERULE R2                                           
                WHERE R2.COMPOSITERULEID = R.RULEID                             
                ) AS ISCOMPOSITE ,                                              
                R.RULEID ,                                                      
                R.TYPE ,                                                        
                R.DESCRIPTION ,                                                 
                R.SEVERITY ,                                                    
                CR.COMPOSITERULEID ,                                            
                CR.TYPE AS COMPOSITERULETYPE ,                                  
                RC.RULECONTEXTID                                                
                FROM RULECONTEXTRULE RCR                                        
                JOIN RULECONTEXT RC                                             
                ON RC.RULECONTEXTID = RCR.RULECONTEXTID                         
                JOIN RULE R                                                     
                ON R.RULEID = RCR.RULEID                                        
                LEFT OUTER JOIN COMPOSITERULERULE CRR                           
                ON R.RULEID = CRR.RULEID                                        
                LEFT OUTER JOIN COMPOSITERULE CR                                
                ON CRR.COMPOSITERULEID = CR.COMPOSITERULEID                     
                WHERE RC.TYPE LIKE P_RULECONTEXT                                
                AND ( CR.COMPOSITERULEID IS NULL                                
                OR CR.COMPOSITERULEID IN                                        
                ( SELECT                                                        
                CR2.COMPOSITERULEID                                             
                FROM COMPOSITERULE CR2                                          
                JOIN RULECONTEXTRULE RCR2                                       
                ON RCR2.RULEID = CR2.COMPOSITERULEID                            
                JOIN RULECONTEXT RC2                                            
                ON RCR2.RULECONTEXTID = RC2.RULECONTEXTID                       
                WHERE RC2.TYPE LIKE P_RULECONTEXT ) )                           
                ORDER BY                                                        
                R.SEVERITY ,                                                    
                CR.COMPOSITERULEID ,                                            
                R.RULEID ;                                                      
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
