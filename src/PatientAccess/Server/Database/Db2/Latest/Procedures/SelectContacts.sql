                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTCONTACTS - SQL PROC FOR PX                     */        
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
                                                                                
CREATE PROCEDURE SELECTCONTACTS (                                               
        IN P_HSP INTEGER ,                                                      
        IN P_MRC INTEGER )                                                      
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELCONT                                                        
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        --emergency contacts                                                    
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        LACNM AS CONTACTNAME ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''         
        LACADR AS ADDRESS ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''            
        LACCIT AS CITY ,  --CHAR(15) CCSID 37 NOT NULL DEFAULT ''               
        LACSTE AS STATE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''               
        LACZPA AS ZIP ,  --CHAR(5) CCSID 37 NOT NULL DEFAULT ''                 
        LACZ4A AS ZIPEXT ,  --CHAR(4) CCSID 37 NOT NULL DEFAULT ''              
        LACRCD AS RELATIONSHIPCODE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''    
        LACACD AS AREACODE ,  --DECIMAL(3, 0) NOT NULL DEFAULT 0                
        LACPH# AS PHONENUMBER    --DECIMAL(7, 0) NOT NULL DEFAULT 0             
        FROM HPADLAP3                                                           
        WHERE LAHSP# = P_HSP                                                    
        AND LAMRC# = P_MRC                                                      
        ORDER BY LACNM ;                                                        
                                                                                
         --nearest relative contacts                                            
        DECLARE CURSOR2 CURSOR FOR                                              
        SELECT                                                                  
        LARNM AS CONTACTNAME ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''         
        LARADR AS ADDRESS ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''            
        LARCIT AS CITY ,  --CHAR(15) CCSID 37 NOT NULL DEFAULT ''               
        LARSTE AS STATE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''               
        LARZPA AS ZIP ,  --CHAR(5) CCSID 37 NOT NULL DEFAULT ''                 
        LARZ4A AS ZIPEXT ,  --CHAR(4) CCSID 37 NOT NULL DEFAULT ''              
        LANRCD AS RELATIONSHIPCODE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''    
        LARACD AS AREACODE ,  --DECIMAL(3, 0) NOT NULL DEFAULT 0                
        LARPH# AS PHONENUMBER    --DECIMAL(7, 0) NOT NULL DEFAULT 0             
        FROM HPADLAP2                                                           
        WHERE LAHSP# = P_HSP                                                    
        AND LAMRC# = P_MRC                                                      
        ORDER BY LARNM ;                                                        
        OPEN CURSOR1 ;                                                          
        OPEN CURSOR2 ;                                                          
        END P1  ;                                                               
