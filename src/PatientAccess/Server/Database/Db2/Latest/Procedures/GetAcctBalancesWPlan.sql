                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETACCTSBALANCESWPLAN - SQL PROC FOR PX              */        
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
                                                                                
CREATE PROCEDURE GETACCTSBALANCESWPLAN (                                        
        IN P_HSP INTEGER ,                                                      
        IN P_HSPCODE CHAR(3) ,                                                  
        IN P_MRC INTEGER )                                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC GETACCBLPL                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
          DECLARE CURSOR1 CURSOR FOR                                            
          SELECT A.LPLDD AS DISCHARGEDATE ,                                     
          A.LPACCT AS ACCOUNTNUMBER ,                                           
          A.LPFC AS FINANCIALCLASSCODE ,                                        
          --t.QTKEY  as PATIENTTYPECODE,                                        
          --t.QTTAD  as PATIENTTYPEDESC,                                        
          A.LPPTYP AS PATIENTTYPECODE ,                                         
          B.PMTDU9 AS TOTALDUE                                                  
          FROM HPADLPUC A                                                       
          LEFT JOIN PM0001P B ON                                                
          ( B.PMHSPC = P_HSPCODE AND B.PMPT#9 = A.LPACCT )                      
          WHERE A.LPHSP# = P_HSP AND  --test use 900                            
          A.LPMRC# = P_MRC AND  --test use  785147                              
          A.LPFC = '73' AND  --test use '80'                                    
          B.PMTDU9 > 0 ;                                                        
          OPEN CURSOR1 ;                                                        
          END P1  ;                                                             
