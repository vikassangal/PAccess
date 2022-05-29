                                                                                  
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTPLANSFORBROKER - SQL PROC FOR PX               */        
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
/* 02/08/2008 I	 Sophie Zhang      I  Left join FacInsuranceOpts (CR) */
/* 02/08/2008 I	 Sophie Zhang      I  Remove hard-coded medicare category id */ 
/* 02/08/2008 I	 Sophie Zhang	   I  Remove '0001-01-01'             */
/* 04/11/2008 I	 Kiran Kumar	   I  Remove Joins with facilities table*/
/*			  I                    I  and changed parameter from @P_FACILITYID to @P_FACILITYCODE*/         
/* 05/06/2008 I  Sanjeev Kumar     I  Replace MPS to use PLANMPSFOURTHCHAR table. SR51063 */
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPLANSFORBROKER(                                                   
        IN @P_BROKERCODE VARCHAR(5),                                                  
        IN @P_PLANCATEGORYID INTEGER ,
        IN @P_FACILITYCODE VARCHAR(10) ,
        IN @P_ADMITDATE DECIMAL(8,0) )                                                 
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPLNBRK                                                        
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
		DECLARE P_TERMPURGEDAYS INTEGER DEFAULT 0;
		DECLARE P_CANXPURGEDAYS INTEGER DEFAULT 0;
		                                                    
		DECLARE CURSOR1 CURSOR FOR                  
                SELECT 
                0 AS PLANID,
		TRIM(PLANS.CTPLI2) AS PLANSUFFIX,
		TRIM(PLANS.CTPLNM) AS PLANNAME,
		0 AS PLANTYPEID,
		TRIM(PLANS.CTPLTY) AS PLANTYPECODE,
		GETCATEGORYTYPE(PLANS.CTPLI2) AS PLANCATEGORYID,
		0 AS PAYORID,
		TRIM(PAYORS.PYPYNM) AS PAYORNAME,
		TRIM(PAYORS.PYPYCD) AS PAYORCODE,
		TRIM(LINESOFBUSINESS.LBDESC) AS LOBDESCRIPTION,
		PLANS.CTCBGD AS PLANEFFECTIVEDATE,
		PLANS.CTPLAD AS PLANAPPROVALDATE,
		PLANS.CTCEND AS PLANTERMINATIONDATE,
		PLANS.CTCCLD AS PLANCANCELDATE,
		P_TERMGRACEDAYS AS TERMGRACEDAYS, 
		P_CANXGRACEDAYS AS CANCELGRACEDAYS,
		P_TERMPURGEDAYS AS TERMPURGEDAYS,
		P_CANXPURGEDAYS AS CANCELPURGEDAYS,
		CASE 
		WHEN PLANS.CTCEND <> 0 
		THEN DATETOCHARYYYYMMDD
		( DATE(INSERT(INSERT(LEFT(CHAR(PLANS.CTCEND),8),5,0,'-'),8,0,'-')) 
		+ P_TERMGRACEDAYS DAYS )
		ELSE 0 
		END AS ADJUSTEDTERMINATIONDATE,
		CASE 
		WHEN PLANS.CTCCLD <> 0 
		THEN DATETOCHARYYYYMMDD
		( DATE(INSERT(INSERT(LEFT(CHAR(PLANS.CTCCLD),8),5,0,'-'),8,0,'-'))
		+ P_CANXGRACEDAYS DAYS )
		ELSE 0
		END AS ADJUSTEDCANCELLATIONDATE
		FROM NKPY25P AS PAYORS
		
		JOIN NKCT01P AS PLANS 
		ON  PAYORS.PYHSPC = PLANS.CTHSPC 
		AND PAYORS.PYPYCD = PLANS.CTPLI3
		
		JOIN NKBK33P AS BROKERS 
		ON PLANS.CTPLBK = BROKERS.BKPLBK 
		AND PLANS.CTHSPC = BROKERS.BKHSPC
		
		JOIN NKLB40P AS LINESOFBUSINESS 
		ON PLANS.CTLNBS = LINESOFBUSINESS.LBLOB
		
		--JOIN NOHLHSPP AS FACILITIES 
		--ON FACILITIES.HHHSPC = PLANS.CTHSPC
		
		WHERE
		-- FACILITIES.HHHSPN = @P_FACILITYID
		PAYORS.PYHSPC = @P_FACILITYCODE
		AND (@P_BROKERCODE = '' OR BROKERS.BKPLBK = @P_BROKERCODE) 
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
				)
		 AND (
				@P_PLANCATEGORYID = 0
				OR
				GETCATEGORYTYPE(PLANS.CTPLI2) = @P_PLANCATEGORYID
				OR
				@P_PLANCATEGORYID = P_MEDCATEGORYID
				AND SUBSTR( PLANS.CTPLI2, 1, 1 ) IN (SELECT PLANIDFIRSTCHAR FROM PLANMPSFOURTHCHAR)
					) 
				ORDER BY PAYORCODE, PLANSUFFIX;                                                 
                
                SELECT PLANCATEGORYID
			INTO P_MEDCATEGORYID
			FROM PLANCATEGORIES
			WHERE UPPER(PLANCATEGORYDESCRIPTION) = 'GOVERNMENT-MEDICARE'; 
                
                SELECT COUNT(*)
			INTO P_COUNT
			FROM NKPT36P FACINSURANCEOPTS
			
			--JOIN NOHLHSPP FACILITIES
			--ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
			WHERE FACINSURANCEOPTS.PTHSPC = @P_FACILITYCODE
			AND FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD';
				
		IF P_COUNT > 0 THEN

			SELECT PTDATA
			INTO P_PTDATA 
			FROM NKPT36P FACINSURANCEOPTS
			
			--JOIN NOHLHSPP FACILITIES 
			--ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
			WHERE FACINSURANCEOPTS.PTHSPC = @P_FACILITYCODE
			AND FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD' 
			FETCH FIRST ROW ONLY;
			
			SET P_TERMGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 7, 3));
			SET P_CANXGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 10, 3));
			SET P_TERMPURGEDAYS = INTEGER(SUBSTR(P_PTDATA, 1, 3));
			SET P_CANXPURGEDAYS = INTEGER(SUBSTR(P_PTDATA, 4, 3));
				
			END IF; 
                
                OPEN CURSOR1 ; 
                                                                 
                END P1  ;                                                       
