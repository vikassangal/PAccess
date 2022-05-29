                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ADTPATIENTFOR - SQL PROC FOR PX                      */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ADTPATIENTFOR (                                                
IN P_HSP INTEGER ,                                                              
IN P_MRC INTEGER ,                                                              
IN P_ACCOUNTNUMBER INTEGER )                                                    
DYNAMIC RESULT SETS 4                                                           
LANGUAGE SQL                                                                    
SPECIFIC ADTPATFOR                                                              
NOT DETERMINISTIC                                                               
MODIFIES SQL DATA                                                               
CALLED ON NULL INPUT                                                            
P1 : BEGIN                                                                      
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        MDDOB8 AS DOB,                                                          
        MDLNGC AS LANGUAGECODE,                                                 
        MDPTID AS NATIONALCODE,                                                 
        MDMAR AS MARITALSTATUSCODE,                                             
        MDRACE AS RACECODE,                                                     
        MDETHC AS ETHNICITYCODE,                                                
        MDPOB AS PLACEOFBIRTH ,                                                 
        MDRLGN AS RELIGIONCODE ,                                                
        MDPARC AS RELIGIOUSCONGREGATIONCODE,                                    
        MDSMOK AS SMOKING,                                                      
        MDNPPV AS NPPVERSION,                                                   
        MDDTRC AS NPPDATE,                                                      
        MDFC AS FINANCIALCLASSCODE,                                             
        MDBLDL AS BLOODLESS,                                                    
        MDSEX AS SEXCODE,                                                       
        MDSSN9 AS SSN,                                                          
        MDDL# AS DRIVERSLICENSENUM,                                             
        MDEMAL AS EMAILADDRESS,                                                 
        MDPLNM AS LASTNAME,                                                     
        MDPFNM AS FIRSTNAME,                                                    
        MDPMI AS MIDDLEINITIAL,                                                 
        MDNMTL AS SUFFIX,                                                       
        MDPADR AS LOCALADDRESS,                                                 
        MDPCIT AS LOCALCITY,                                                    
        MDPCCD AS COUNTYCODE,                                                   
        MDPSTE AS LOCALSTATECODE,                                               
        MDPZPA AS LOCALPOSTALCODE,                                              
        MDPZ4A AS LOCALPOSTALEXT,                                               
        MDPCUN AS COUNTRYCODE,                                                  
        MDPACD AS LOCALPHONEAREACODE,                                           
        MDPPH# AS LOCALPHONENUMBER,                                             
        MDCLPH AS CELLPHONENUMBER,                                              
        MDMADR  AS      PERMANENTADDRESS,                                       
        MDMCIT  AS      PERMANENTCITY,                                          
        MDMSTE  AS      PERMANENTSTATECODE,                                     
        MDMZPA  AS      PERMANENTPOSTALCODE,                                    
        MDMZ4A  AS      PERMANENTPOSTALEXT,                                     
        MDMACD  AS      PERMANENTPHONEAREACODE,                                 
        MDMPH#  AS      PERMANENTPHONENUMBER                                    
        FROM HPADMDP P                                                          
        WHERE P . MDHSP# = P_HSP                                                
        AND P . MDMRC# = P_MRC ;                                                
                                                                                
        DECLARE CURSOR2 CURSOR FOR                                              
        SELECT                                                                  
        G . MGGSEX AS SEXCODE ,                                                 
        G . MGGSSN      AS SSN,                                                 
        G . MGDRL#      AS      DRIVERSLICENSE,                                 
        G . MGEMAL      AS      EMAILADDRESS,                                   
        G . MGGLNM      AS      LASTNAME,                                       
        G . MGGFNM      AS      FIRSTNAME,                                      
        G . MGGMI       AS      MIDDLEINITIAL,                                  
        G . MGGAD1      AS      LOCALADDRESS,                                   
        G . MGGCIT      AS      LOCALCITY,                                      
        G . MGGCNY      AS      COUNTYCODE,                                     
        G . MGGSTE      AS      LOCALSTATECODE,                                 
        G . MGGZPA      AS      LOCALPOSTALCODE,                                
        G . MGGZ4A      AS      LOCALPOSTALEXT,                                 
        G . MGGCUN      AS      COUNTRYCODE,                                    
        G . MGGACD      AS      LOCALPHONEAREACODE,                             
        G . MGGPH#      AS      LOCALPHONENUMBER,                               
        G.  MGCLPH  AS  CELLPHONENUMBER,                                        
        L . LPGAR# AS GUARANTORNUMBER,                                          
        L . LPRRCD AS RELATIONSHIPTOGUARANTOR                                   
        FROM HPADMGP G                                                          
        JOIN HPADLPP L ON ( G . MGGAR# = L . LPGAR# )                           
        AND ( L . LPHSP# = G . MGHSP# )                                         
        WHERE ( G . MGHSP# = P_HSP )                                            
        AND ( L . LPMRC# = P_MRC )                                              
        AND ( L . LPACCT = P_ACCOUNTNUMBER )                                    
        AND ( L . LPHSP# = G . MGHSP# ) ;                                       
        DECLARE CURSOR3 CURSOR FOR                                              
        SELECT                                                                  
        LAWNM   AS EMPLOYERNAME ,                                               
        LAWADR  AS ADDRESS,                                                     
        LAWACD  AS PHONEAREACODE,                                               
        LAWPH#  AS PHONENUMBER,                                                 
        LAWCIT  AS CITY,                                                        
        LAWSTE  AS STATECODE,                                                   
        LAESCD  AS EMPLOYMENTSTATUSCODE,                                        
        LAEEID  AS EMPLOYEEID,                                                  
        LAPOCC  AS OCCUPATION,                                                  
        LAWZPA  AS POSTALCODE,                                                  
        LAWZ4A  AS POSTALEXT,                                                   
        LAWCUN  AS COUNTRYCODE                                                  
        FROM HPADLAP5 E                                                         
        WHERE ( E . LAHSP# = P_HSP )                                            
        AND ( E . LAMRC# = P_MRC ) ;                                            
        DECLARE CURSOR4 CURSOR FOR                                              
        SELECT                                                                  
        AKA . AKPLNM    AS      PATIENTLASTNAME ,                               
        AKA . AKPFNM    AS      PATIENTFIRSTNAME,                               
        AKA . AKTITL    AS      TITLE,                                          
        AKA . AKEDAT    AS      ENTRYDATE,                                      
        AKA . AKSECF    AS      SECURITYFLAG                                    
        FROM NMNHAKAP AKA                                                       
        WHERE ( AKA . AKHSP# = P_HSP )                                          
        AND ( AKA . AKMRC# = P_MRC ) ;                                          
        OPEN CURSOR1 ;                                                          
        OPEN CURSOR2 ;                                                          
        OPEN CURSOR3 ;                                                          
        OPEN CURSOR4 ;                                                          
END P1  ;                                                                       
