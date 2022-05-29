                                                                                 
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLPAYORBROKERS - SQL PROC FOR PX              */        
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
/* 01/23/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */
/* 02/08/2008 I	 Sophie Zhang      I  Left join FacInsuranceOpts (CR  */
/* 02/08/2008 I	 Sophie Zhang      I  Remove hard-coded medicare category id */       
/* 03/18/2008 I Kevin Sedota       I  start using search names        */
/* 04/11/2008 I	 Kiran Kumar	   I  Remove Joins with facilities table*/
/*			  I                    I  and changed parameter from @P_FACILITYID to @P_FACILITYCODE*/
/* 05/06/2008 I  Sanjeev Kumar     I  Replace MPS to use PLANMPSFOURTHCHAR table. SR51063 */
/* 10/06/2008 I  Smitha Krishnam   I  Replace PLANMPSFOURTHCHAR with IN list. OTD37267 */
/* 10/06/2008 I  Jithin A	       I  Modified the name & made it category specific procedure  */
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTALLPAYORBROKERSWITH( 
	IN @P_INSNAME VARCHAR(20) ,                                                  
        IN @P_PLANCATEGORYID INTEGER ,
        IN @P_FACILITYCODE VARCHAR(10) ,
        IN @P_SEARCHOPTION INTEGER ,
        IN @P_ADMITDATE DECIMAL(8,0) )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SLPYRBRKW                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN
	DECLARE P_MEDCATEGORYID INTEGER DEFAULT 3;
	DECLARE P_COUNT INTEGER DEFAULT 0;
	DECLARE P_PTDATA VARCHAR(74) DEFAULT '';
	DECLARE P_TERMGRACEDAYS INTEGER DEFAULT 0;
	DECLARE P_CANXGRACEDAYS INTEGER DEFAULT 0;
	      
	DECLARE CURSOR1 CURSOR FOR 
	SELECT PROVIDERID, 
	PROVIDERNAME, 
	PROVIDERCODE, 
	PROVIDERTYPE, 
	ACTIVEPLANCOUNT
	FROM
	(                 
        SELECT 
        0 AS PROVIDERID,
        TRIM(PAYORS.PYPYNM) AS PROVIDERNAME,
        TRIM(PAYORS.PYPYCD) AS PROVIDERCODE,
        'P' AS PROVIDERTYPE,
        COUNT(1) AS ACTIVEPLANCOUNT
	FROM NKPY25P AS PAYORS
		JOIN NKCT01P AS PLANS 
		ON  PAYORS.PYHSPC = PLANS.CTHSPC 
		AND PAYORS.PYPYCD = PLANS.CTPLI3
		WHERE PAYORS.PYHSPC = @P_FACILITYCODE
		AND (	
		PLANISVALID(@P_ADMITDATE, 
			PLANS.CTPLAD,
			PLANS.CTCBGD,
			PLANS.CTCEND,
			PLANS.CTCCLD,
			P_TERMGRACEDAYS,
			P_CANXGRACEDAYS,
			PLANS.CTPLI2
			) = 1  
		AND 
		(
			( @P_SEARCHOPTION = 0 
			AND UPPER(PAYORS.PYPYSN) 
			LIKE UPPER(@P_INSNAME) || '%'
				OR
			@P_SEARCHOPTION = 1 
			AND UPPER(PAYORS.PYPYSN) 
			LIKE '%' || UPPER(@P_INSNAME) || '%'
			)
			AND
			(GETCATEGORYTYPE(PLANS.CTPLI2) = @P_PLANCATEGORYID )
			OR
			@P_PLANCATEGORYID = P_MEDCATEGORYID 
			AND SUBSTR (PLANS.CTPLI2, 1, 1 ) IN  ('6','I','O','F','S','W','E') 
		)  

				)				
	GROUP BY PAYORS.PYPYCD, PAYORS.PYPYNM
			
	UNION ALL	
			
	SELECT 
        0 AS PROVIDERID,
        TRIM(BROKERS.BKBKNM) AS PROVIDERNAME,
        TRIM(BROKERS.BKPLBK) AS PROVIDERCODE,
        'B' AS PROVIDERTYPE,
        COUNT(1) AS ACTIVEPLANCOUNT
			FROM NKBK33P AS BROKERS
			JOIN NKCT01P AS PLANS 
			ON  BROKERS.BKHSPC = PLANS.CTHSPC 
			AND BROKERS.BKPLBK = PLANS.CTPLBK
			WHERE BROKERS.BKHSPC = @P_FACILITYCODE
			AND (	
			PLANISVALID(@P_ADMITDATE, 
				PLANS.CTPLAD,
				PLANS.CTCBGD,
				PLANS.CTCEND,
				PLANS.CTCCLD,
				P_TERMGRACEDAYS,
				P_CANXGRACEDAYS,
				PLANS.CTPLI2
				) = 1	
			AND 
			(
			( @P_SEARCHOPTION = 0 
			AND UPPER(BROKERS.BKBKSN) 
			LIKE UPPER(@P_INSNAME) || '%'
				OR
			  @P_SEARCHOPTION = 1 
			  AND UPPER(BROKERS.BKBKSN)
			  LIKE '%' || UPPER(@P_INSNAME) || '%'
			 )
			 AND
			(  GETCATEGORYTYPE(PLANS.CTPLI2) = @P_PLANCATEGORYID )
			OR
			@P_PLANCATEGORYID = P_MEDCATEGORYID 
			AND SUBSTR (PLANS.CTPLI2, 1, 1 ) IN  ('6','I','O','F','S','W','E') 
			)  
					)				
				GROUP BY BROKERS.BKPLBK, 
				BROKERS.BKBKNM
			) AS V(PROVIDERID, 
			PROVIDERNAME, 
			PROVIDERCODE, 
			PROVIDERTYPE, 
			ACTIVEPLANCOUNT)	
			ORDER BY PROVIDERNAME;                                                 
        
        SELECT PLANCATEGORYID
			INTO P_MEDCATEGORYID
			FROM PLANCATEGORIES
			WHERE UPPER(PLANCATEGORYDESCRIPTION) = 'GOVERNMENT-MEDICARE'; 
			
			SELECT COUNT(1)
			INTO P_COUNT
			FROM NKPT36P FACINSURANCEOPTS
		
			WHERE FACINSURANCEOPTS.PTHSPC = @P_FACILITYCODE
			AND FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD';
			
			IF P_COUNT > 0 THEN

			SELECT PTDATA
			INTO P_PTDATA 
			FROM NKPT36P FACINSURANCEOPTS

			WHERE FACINSURANCEOPTS.PTHSPC = @P_FACILITYCODE
			AND FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD' 
			FETCH FIRST ROW ONLY;
			
			SET P_TERMGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 7,3));
			SET P_CANXGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 10,3));
			
			END IF;
			
			OPEN CURSOR1 ; 
                                                         
        END P1  ;                                                       
 