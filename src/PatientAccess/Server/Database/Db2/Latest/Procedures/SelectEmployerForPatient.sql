 /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTEMPLOYERSFORPATIENT - SQL PROC FOR PX           */        
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
/* 01/31/2008 I  Sophie Zhang      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        

SET PATH *LIBL ;                                                                

CREATE PROCEDURE SELECTEMPLOYERFORPATIENT(                                         
          IN @P_MRCNUMBER DECIMAL(9),
          IN @P_FACILITYID	DECIMAL(3))                                               
                                                                                
          DYNAMIC RESULT SETS 1                                                 
          LANGUAGE SQL                                                          
          SPECIFIC SELEMPTNT                                                     
          NOT DETERMINISTIC                                                     
          READS SQL DATA                                                        
   P1 : BEGIN                                                                   
     DECLARE CURSOR1 CURSOR FOR                                                 
        SELECT 
			TRIM(PE.LAWNM) AS EMPLOYERNAME, 
			TRIM(PE.LAWADR) AS ADDRESS1,
			''	AS ADDRESS2, 
			TRIM(PE.LAWZPA) || TRIM(PE.LAWZ4A) AS POSTALCODE,
			TRIM(PE.LAWCIT) AS CITY ,
			TRIM(PE.LAWSTE) AS STATECODE,
			TRIM(PE.LAWCUN) AS COUNTRYCODE,
			1 AS PHONECOUNTRYCODE,
			PE.LAWACD AS AREACODE,    -- DECIMAL
			PE.LAWPH# AS PHONENUMBER,      -- DECIMAL
			'' AS EMAILADDRESS,
			1 AS TYPEOFCONTACTPOINTID,
			'EMPLOYER' AS CONTACTPOINTDESCRIPTION,
			TRIM(PE.LAESCD) AS EMPLOYMENTSTATUSCODE,
			TRIM(PE.LAEEID) AS EMPLOYEEID,
			TRIM(PE.LAPOCC) AS OCCUPATION
        FROM HPADLAP5 PE
        WHERE PE.LAHSP# = @P_FACILITYID AND PE.LAMRC# = @P_MRCNUMBER; 
           OPEN CURSOR1 ;                                                       
 END P1  ;                                                                      
