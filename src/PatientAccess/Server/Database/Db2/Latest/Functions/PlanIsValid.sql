 /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PLANISVALID - SQL FUNCTION FOR PX		      */        
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
/* 01/21/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */ 
/* 02/07/2008 I	 Sophie Zhang	   I  take care of ADMITDATE=0        */       
/*************I********************I***********************************/     
 
SET PATH *LIBL ; 

CREATE FUNCTION PLANISVALID ( 
	ADMITDATE DECIMAL(8,0),
	APPROVALDATE DECIMAL(8,0),
	EFFECTIVEDATE DECIMAL(8,0),
	TERMINATIONDATE DECIMAL(8,0),
	CANCELLATIONDATE DECIMAL(8,0),
	DAYSTOREMAFTERTERM INTEGER,
	DAYSTOREMAFTERCANX INTEGER,
	PLANSUFFIX VARCHAR(2) ) 
	RETURNS INTEGER   
	LANGUAGE SQL 
	SPECIFIC PLANVALID 
	DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	NO EXTERNAL ACTION 

	BEGIN
		DECLARE ADJUSTEDTERMINATIONDATE DATE DEFAULT NULL;
		DECLARE ADJUSTEDCANCELLATIONDATE DATE DEFAULT NULL;
		DECLARE ADJUSTEDTERMINATIONDATECHAR VARCHAR(10) DEFAULT '';
		DECLARE ADJUSTEDCANCELLATIONDATECHAR VARCHAR(10) DEFAULT '';
		DECLARE ADJUSTEDTERMINATION DECIMAL(8,0) DEFAULT 0;
		DECLARE ADJUSTEDCANCELLATION DECIMAL(8,0) DEFAULT 0; 
		 
		IF (TERMINATIONDATE <> 0)
		THEN 
			SET ADJUSTEDTERMINATIONDATE = 
				DATE(INSERT(INSERT(LEFT(CHAR(TERMINATIONDATE)
				,8),5,0,'-'),8,0,'-'))
				+ DAYSTOREMAFTERTERM DAY;
			SET ADJUSTEDTERMINATIONDATECHAR = CHAR(ADJUSTEDTERMINATIONDATE);
			SET ADJUSTEDTERMINATION = DECIMAL(SUBSTRING
				(ADJUSTEDTERMINATIONDATECHAR, 1, 4) || 
				SUBSTRING(ADJUSTEDTERMINATIONDATECHAR, 6, 2) ||
				SUBSTRING(ADJUSTEDTERMINATIONDATECHAR, 9, 2)); 
		END IF; 
		
		IF (CANCELLATIONDATE <> 0)
		THEN
			SET ADJUSTEDCANCELLATIONDATE = 
				DATE(INSERT(INSERT(LEFT(CHAR(CANCELLATIONDATE)
				,8),5,0,'-'),8,0,'-'))
				+ DAYSTOREMAFTERCANX DAY;	
			SET ADJUSTEDCANCELLATIONDATECHAR = CHAR(ADJUSTEDCANCELLATIONDATE);	
			SET ADJUSTEDCANCELLATION = DECIMAL(SUBSTRING
				(ADJUSTEDCANCELLATIONDATECHAR, 1, 4) ||
				SUBSTRING(ADJUSTEDCANCELLATIONDATECHAR, 6, 2) ||
				SUBSTRING(ADJUSTEDCANCELLATIONDATECHAR, 9, 2));
		END IF;

		IF ( APPROVALDATE <> 0 
			AND EFFECTIVEDATE <> 0 
			AND ( ADMITDATE = 0 OR EFFECTIVEDATE <= ADMITDATE)
			AND (TERMINATIONDATE = 0 OR ADJUSTEDTERMINATION >= ADMITDATE ) 
			AND (CANCELLATIONDATE = 0 OR ADJUSTEDCANCELLATION >= ADMITDATE)
			AND PLANSUFFIX <> '00'
			AND LEFT(PLANSUFFIX, 1) <> 'Z' )
		THEN 
			RETURN 1;
		ELSE 
			RETURN 0;
		END IF;
		END;
			
