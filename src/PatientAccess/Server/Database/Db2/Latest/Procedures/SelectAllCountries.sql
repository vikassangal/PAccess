                                                                                  
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLCOUNTRIES - SQL PROC FOR PX        */        
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
                                                                                
CREATE PROCEDURE SELECTALLCOUNTRIES()                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELALLCNRY                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                
        DECLARE CURSOR1 CURSOR FOR          
                                            
            SELECT COUNTRYID as OID, COUNTRYCODE as CODE, COUNTRYDESCRIPTION as DESCRIPTION
            FROM            
				(
					SELECT 	DISTINCT
							0 AS COUNTRYID,
							TRIM(QTKEY) AS COUNTRYCODE,
							TRIM(QTCNTD) AS COUNTRYESCRIPTION
					FROM	HPADQTCY
					WHERE	QTHSP# = 999
					AND		QTKEY IS NOT NULL
					AND		QTKEY <> ''

				UNION ALL

					SELECT	0 AS COUNTRYID,
							'' AS COUNTRYCODE,
							'' AS COUNTRYDESCRIPTION
					FROM	SYSIBM/SYSDUMMY1

				) AS V(COUNTRYID, COUNTRYCODE, COUNTRYDESCRIPTION)

				ORDER BY CASE COUNTRYCODE
						WHEN ' '	THEN	1
						WHEN 'USA'	THEN	2
						WHEN 'CAN'	THEN	3
						WHEN 'MEX'	THEN	4
						ELSE				99
				END;

        OPEN CURSOR1 ;

        END P1  ;
