/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTEMPLOYERSFORGUARANTOR - SQL PROC FOR PX         */        
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
/* 02/01/2008 I  Sophie Zhang      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        

SET PATH *LIBL ;                                                                

CREATE PROCEDURE SELECTEMPLOYERFORGUARANTOR(                                         
          IN @P_ACCOUNTNUMBER DECIMAL(9),
          IN @P_FACILITYID	DECIMAL(3))                                               
                                                                                
          DYNAMIC RESULT SETS 1                                                 
          LANGUAGE SQL                                                          
          SPECIFIC SELEMPGRNT                                                     
          NOT DETERMINISTIC                                                     
          READS SQL DATA                                                        
   P1 : BEGIN                                                                   
     DECLARE CURSOR1 CURSOR FOR                                                 
          SELECT 
			TRIM(GE.LAENM) AS EMPLOYERNAME, 
			TRIM(GE.LAEADR) AS ADDRESS1,
			'' AS ADDRESS2, 
			TRIM(GE.LAEZPA) || TRIM(GE.LAEZ4A) AS POSTALCODE,
			TRIM(GE.LAECIT) AS CITY ,
			TRIM(GE.LAESTE) AS STATECODE,
			TRIM(GE.LAECUN) AS COUNTRYCODE,
			1 AS PHONECOUNTRYCODE,
			GE.LAEACD AS AREACODE,    -- DECIMAL
			GE.LAEPH# AS PHONENUMBER,      -- DECIMAL
			'' AS EMAILADDRESS,
			1 AS TYPEOFCONTACTPOINTID,
			'EMPLOYER' AS CONTACTPOINTDESCRIPTION,
			TRIM(GE.LAGESC) AS EMPLOYMENTSTATUSCODE,
			TRIM(GE.LAGEID) AS EMPLOYEEID,
			TRIM(GE.LAGOCC) AS OCCUPATION
        FROM HPADLAP1 GE
        WHERE GE.LAHSP# = @P_FACILITYID AND GE.LAGAR# = @P_ACCOUNTNUMBER;                           
        OPEN CURSOR1 ;                                                       
 END P1  ;                                                                      
