                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTNURSINGSTATIONSFOR - SQL PROC FOR PX           */        
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
                                                                                
CREATE PROCEDURE SELECTNURSINGSTATIONSFOR (                                     
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELNSFOR                                                       
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        LNNS AS NURSINGSTATIONCODE ,                            
                        LNDE20 AS NURSINGSTATIONDESCRIPTION ,                   
                        LNCPCN AS PREVIOUSCENSUS ,                              
                        LNCADM AS ADMITTODAY ,                                  
                        LNCDIS AS DISCHARGETODAY ,                              
                        LNCDTH AS DEATHSTODAY ,                                 
                        LNCTRF AS TRANSFERREDFROMTODAY ,                        
                        LNCTRT AS TRANSFERREDTOTODAY ,                          
                        LNCBAV AS AVAILABLEBEDS ,                               
                        LNCBDS AS TOTALBEDS ,                                   
                        LNCMOC AS TOTALOCCUPIEDBEDSFORMONTH ,                   
                        LNCMTB AS TOTALBEDSFORMONTH,
                        LNSATT AS SITECODE                             
                        FROM HPADLNP                                            
                        WHERE LNHSP# = P_FACILITYID                             
                        AND ( LNRBFL = 'Y' OR LNNS = '$$' )                     
                        ORDER BY LNNS ;                                         
                                                                                
                                OPEN CURSOR1 ;                                  
                                                                                
                                END P1  ;                                       
