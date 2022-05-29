                                                                                
/**********************************************************************/        
/* AS400 Short Name: ACCOM00004                                       */        
/* iSeries400        ACCOMODATIONCODESFOR4 -SQL PROC                  */        
/*                                          FOR PATIENT ACCESS        */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *                                                          *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods and techniques described herein are          *    */        
/*    * considered trade secrets and/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*                                                                    */        
/*                                                                    */        
/***********I**********I***********I***********************************/        
/*  Date    I Request #I  Pgmr     I  Modification Description        */        
/***********I**********I***********I***********************************/        
/* 05/17/06 I 4043030  I  D Evans  I NEW PROC                         */
/* 06/26/07 I OTD 34372   K Sedota I Trim the nursing station ID      */
/*                                   Because the this is trimmed in   */
/*                                   UI input.                        */        
/***********I**********I***********I***********************************/        
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ACCOMODATIONCODESFOR4 (                                        
        IN P_NURSINGSTATION VARCHAR(3) ,                                        
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ACMCODFOR4                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
                                                                                
        SELECT  AC . QTKEY AS ACCOMODATIONKEY ,                                 
        AC . QTACCD AS ACCOMODATIONDESC                                         
        FROM HPADQTAC AC                                                        
        JOIN HPOCMCP H ON ( AC . QTHSP# = H . MCHSP#                            
        AND AC . QTKEY >= '00' AND AC . QTKEY <= '99'                           
        AND CAST ( AC . QTKEY AS NUMERIC ( 2 ) ) = H . MCITM# )                 
        AND H . MCINA = 'A'                                                     
        JOIN HPADLNP A ON ( AC . QTHSP# = A . LNHSP#                            
        AND H . MCDEPT = A . LNDEPT )                                           
        WHERE   AC . QTHSP#     = P_FACILITYID                                  
        AND     trim(A . LNNS)        = trim(P_NURSINGSTATION) ;                            
                                                                                
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
