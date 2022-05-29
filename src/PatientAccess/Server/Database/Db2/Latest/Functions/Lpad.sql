                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  LPAD - SQL FUNCTION FOR PX                           */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW FUNC                         */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE FUNCTION LPAD (                                                          
        C1 VARCHAR(4000) ,                                                      
        N INTEGER ,                                                             
        C2 VARCHAR(4000) )                                                      
        RETURNS VARCHAR(4000)                                                   
        LANGUAGE SQL                                                            
        SPECIFIC LPAD                                                           
        DETERMINISTIC                                                           
        CONTAINS SQL                                                            
        CALLED ON NULL INPUT                                                    
        NO EXTERNAL ACTION                                                      
        RETURN                                                                  
        CASE                                                                    
        WHEN N > LENGTH ( C1 ) THEN                                             
        SUBSTR ( REPEAT ( C2 , ( N - LENGTH ( C1 ) +                            
        LENGTH ( C2 ) ) / ( LENGTH ( C2 ) + 1 -                                 
        SIGN ( LENGTH ( C2 ) ) ) ) , 1 , N - LENGTH ( C1 ) ) || C1              
        ELSE SUBSTR ( C1 , 1 , N )                                              
        END  ;                                                                  
                                                                                
