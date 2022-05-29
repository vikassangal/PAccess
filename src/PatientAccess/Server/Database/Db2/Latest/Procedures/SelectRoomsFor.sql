                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTROOMSFOR - SQL PROC FOR PX             */                
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
                                                                                
CREATE PROCEDURE SELECTROOMSFOR (                                               
        IN P_FACILITYID INTEGER ,                                               
        IN P_NURSINGSTATION CHAR(2) )                                           
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELROOMFOR                                                     
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                                DECLARE CURSOR1 CURSOR FOR                      
                                SELECT                                          
                                DISTINCT A.LRROOM AS ROOM                       
                                FROM HPADLRP A                                  
                                WHERE A.LRHSP# = P_FACILITYID                   
                                AND A.LRNS = P_NURSINGSTATION                   
                                ORDER BY A.LRROOM ;                             
                                                                                
                                OPEN CURSOR1 ;                                  
                                                                                
                                END P1  ;                                       
