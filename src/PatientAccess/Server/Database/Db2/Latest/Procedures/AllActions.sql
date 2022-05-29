                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ALLACTIONS - SQL PROC FOR PATIENT ACCESS             */        
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
                                                                                
 CREATE PROCEDURE ALLACTIONS ( )                                                
    DYNAMIC RESULT SETS 1                                                       
    LANGUAGE SQL                                                                
    SPECIFIC ALLACT                                                             
    NOT DETERMINISTIC                                                           
    READS SQL DATA                                                              
    CALLED ON NULL INPUT                                                        
    SET OPTION DBGVIEW = *SOURCE                                                
    P1 : BEGIN                                                                  
                                                                                
      DECLARE CURSOR1 CURSOR FOR                                                
         SELECT                                                                 
         A.ACTIONID ,                                                           
         A.TYPE AS TYPE ,                                                       
         A.DESCRIPTION ,                                                        
         CAA.COMPOSITEACTIONID ,                                                
         ( SELECT                                                               
         CASE WHEN COUNT ( A2.COMPOSITEACTIONID ) > 0 THEN 1                    
         ELSE 0 END                                                             
         FROM COMPOSITEACTION A2                                                
         WHERE A2.COMPOSITEACTIONID = A.ACTIONID                                
         ) AS ISCOMPOSITE                                                       
         FROM ACTION A                                                          
         LEFT OUTER JOIN COMPOSITEACTIONACTION                                  
                  CAA ON A.ACTIONID = CAA.ACTIONID                              
         ORDER BY ACTIONID ;                                                    
                                                                                
         OPEN CURSOR1 ;                                                         
                                                                                
         END P1  ;                                                              
