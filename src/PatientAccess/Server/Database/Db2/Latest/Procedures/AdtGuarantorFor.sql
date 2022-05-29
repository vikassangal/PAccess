                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ADTGUARANTORFOR - SQL PROC FOR PATIENT ACCESS          */      
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
                                                                                
CREATE PROCEDURE ADTGUARANTORFOR (                                              
        IN P_HSP INTEGER ,                                                      
        IN P_MRC INTEGER ,                                                      
        IN P_ACCOUNTNUMBER INTEGER )                                            
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ADTGUAFOR                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
        DECLARE CURSOR1 CURSOR FOR                                              
         SELECT                                                                 
         G.MGGSEX AS SEXCODE ,                                                  
         G.MGGSSN AS SSN ,                                                      
         G.MGDRL# AS DRIVERSLICENSE,                                            
         G.MGEMAL AS EMAILADDRESS,                                              
         G.MGGLNM AS LASTNAME,                                                  
         G.MGGFNM AS FIRSTNAME,                                                 
         G.MGGMI  AS MIDDLEINITIAL,                                             
         G.MGGAD1 AS ADDRESS,                                                   
         G.MGGCIT AS CITY,                                                      
         G.MGGCNY AS COUNTYCODE,                                                
         G.MGGSTE AS STATECODE,                                                 
         G.MGGZIP AS POSTALCODE,                                                
         G.MGGZP4 AS POSTALEXT,                                                 
         G.MGGCUN AS COUNTRYCODE,                                               
         G.MGGACD AS AREACODE,                                                  
         G.MGGPH# AS PHONENUMBER,                                               
         L.LPGAR# AS GUARANTORNUMBER,                                           
         L.LPGREL AS RELATIONSHIPTOGUARANTOR                                    
         FROM HPADMGP G                                                         
         JOIN HPADLPP L ON ( G.MGGAR# = L.LPGAR# )                              
         AND ( L.LPHSP# = G.MGHSP# )                                            
         WHERE ( G.MGHSP# = P_HSP )                                             
         AND ( L.LPMRC# = P_MRC )                                               
         AND ( L.LPACCT = P_ACCOUNTNUMBER )                                     
         AND ( L.LPHSP# = G.MGHSP# ) ;                                          
         OPEN CURSOR1 ;                                                         
         END P1  ;                                                              
