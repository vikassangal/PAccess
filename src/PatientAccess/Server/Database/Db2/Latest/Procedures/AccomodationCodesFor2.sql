/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCOUNTFOR           - SQL PROC FOR PATIENT ACCESS   */        
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
/*                                                                    */        
/***********I**********I***********I***********************************/        
/*  Date    I Request #I  Pgmr     I  Modification Description        */        
/***********I**********I***********I***********************************/        
/* 05/17/06 I 4043030  I  D Evans  I NEW PROC                         */        
/***********I**********I***********I***********************************/        
                                                                                
 SET PATH *LIBL ;                                                               
                                                                                
 CREATE PROCEDURE ACCOMODATIONCODESFOR2 (                                       
        IN P_NURSINGSTATION VARCHAR(3) ,                                        
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ACMCODFOR2                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
                                                                                
        SELECT  AC . QTKEY AS ACCOMODATIONKEY ,                                 
        AC . QTACCD AS ACCOMODATIONDESC                                         
        FROM HPADQTAC AC                                                        
        JOIN (  SELECT  MCHSP# , MCDEPT , MCITM#                                
        FROM    HPOCMCP                                                         
        WHERE   MCHSP#  = P_FACILITYID ) AS H                                   
        ON ( AC . QTHSP# = H . MCHSP# AND CAST                                  
        ( AC . QTKEY AS NUMERIC ( 2 ) ) = H . MCITM# )                          
        JOIN HPADLNP A ON ( AC . QTHSP# = A . LNHSP#                            
                        AND H . MCDEPT = A . LNDEPT )                           
        WHERE   AC . QTHSP#     = P_FACILITYID                                  
        AND     A . LNNS        = P_NURSINGSTATION ;                            
                                                                                
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
