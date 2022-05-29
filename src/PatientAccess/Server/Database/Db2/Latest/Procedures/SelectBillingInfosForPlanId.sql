                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTBILLINGINFOSFORPLANID - SQL PROC FOR PX        */        
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
/* 01/15/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTBILLINGINFOSFORPLANID(                                                   
        IN @P_PLANCD VARCHAR(5) ,
        IN @P_FACILITYID INTEGER,
        IN @P_EFFECTIVEDATE DECIMAL(8,0),
        IN @P_APPROVALDATE DECIMAL(8,0) )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELBLPL                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT 
                0 AS PLANID,
                0 AS CONTACTPOINTID,
                'BILLING' AS CONTACTDESCRIPTION ,
                '' AS EMAILADDRESS ,
                0 AS ADDRESSID,
                TRIM(PLANCONTACTPOINTS.BLPBA1) AS ADDRESS1 ,                                                         
                TRIM(PLANCONTACTPOINTS.BLPBA2) AS ADDRESS2 ,                                                         
                TRIM(PLANCONTACTPOINTS.BLPBCI) AS CITY ,                                                        
                0 AS COUNTYID, 
                0 AS STATEID, 
                TRIM(PLANCONTACTPOINTS.BLPBST) AS STATECODE ,
                TRIM(PLANCONTACTPOINTS.BLPBZP) AS POSTALCODE ,
                0 AS COUNTRYID,
                'USA' AS COUNTRYCODE ,
                0 AS PHONEID ,
                '1' AS PHONECOUNTRYCODE ,
                SUBSTR(CHAR(PLANCONTACTPOINTS.BLPBP1), 1, 3) AS PHONEAREACODE,
                SUBSTR(CHAR(PLANCONTACTPOINTS.BLPBP1), 4, 7) AS PHONENUMBER ,
                2 AS TYPEOFADDRESSID ,
                'BILLING' AS ADDRESSDESCRIPTION ,
                TRIM(PLANCONTACTPOINTS.BLPBCV) AS BILLINGAREA ,
                TRIM(PLANCONTACTPOINTS.BLPBCN) AS BILLINGCONAME ,                                                        
                TRIM(PLANCONTACTPOINTS.BLPYBL) AS BILLINGNAME                                                          
                FROM NKBL09P PLANCONTACTPOINTS
                JOIN NOHLHSPP FACILITIES ON FACILITIES.HHHSPC = PLANCONTACTPOINTS.BLHSPC                                                  
                WHERE PLANCONTACTPOINTS.BLPLID = @P_PLANCD
                AND PLANCONTACTPOINTS.BLCBGD = @P_EFFECTIVEDATE
                AND PLANCONTACTPOINTS.BLPLAD = @P_APPROVALDATE
                AND FACILITIES.HHHSPN = @P_FACILITYID;                                        
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
