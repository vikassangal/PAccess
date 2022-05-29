                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ACCOMODATIONCODESFOR - SQL PROC FOR PATIENT ACCESS   */        
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
                                                                                
   CREATE PROCEDURE ACCOMODATIONCODESFOR (                                      
     IN P_NURSINGSTATION VARCHAR(3) ,                                           
     IN P_FACILITYID INTEGER )                                                  
     DYNAMIC RESULT SETS 1                                                      
     LANGUAGE SQL                                                               
     SPECIFIC ACMCODFOR                                                         
     NOT DETERMINISTIC                                                          
     READS SQL DATA                                                             
     CALLED ON NULL INPUT                                                       
     SET OPTION DBGVIEW = *SOURCE                                               
     P1 : BEGIN                                                                 
        DECLARE DEPTNO VARCHAR ( 10 );                                          
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT AC.QTKEY AS ACCOMODATIONKEY,                                     
        AC.QTACCD AS ACCOMODATIONDESC                                           
        FROM HPADQTAC AC                                                        
        JOIN ( SELECT MCHSP#, MCDEPT,                                           
        SUBSTR ( LPAD ( VARCHAR ( MCITM# ) , 4 , '0' ) , 3 , 2 )                
        AS QTKEY                                                                
        FROM HPOCMCP                                                            
        WHERE MCHSP# = P_FACILITYID )                                           
        AS H ON ( H.MCHSP# = AC.QTHSP# AND AC.QTKEY = H.QTKEY )                 
        JOIN HPADLNP                                                            
        A ON ( AC.QTHSP# = A.LNHSP# AND H.MCDEPT = A.LNDEPT )                   
        WHERE AC.QTHSP# = P_FACILITYID                                          
        AND A.LNNS = P_NURSINGSTATION;                                          
                OPEN CURSOR1;                                                   
                END P1;                                                         
