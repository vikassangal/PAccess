/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTPLANFORPAYORPLANCODE - SQL PROC FOR PX        */        
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
/* 01/16/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */
/* 02/08/2008 I	 Sophie Zhang      I  Left join FacInsuranceOpts (CR) */ 
/* 02/08/2008 I	 Sophie Zhang	   I  Remove '0001-01-01'             */
/* 04/11/2008 I	 Kiran Kumar	   I  Remove Joins with facilities table*/
/*			  I                    I  and changed parameter from @P_FACILITYID to @P_FACILITYCODE*/        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ; 
                                                                                
CREATE PROCEDURE SELECTPLANFORPAYORPLANCODE(                                                   
        IN @P_PLANCD VARCHAR(5) ,
        IN @P_FACILITYCODE VARCHAR(10) )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPLNCD                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN 
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
	FROM NKPY25P PAYORS
		JOIN NKCT01P PLANS 
		ON  PAYORS.PYHSPC = PLANS.CTHSPC 
		AND PAYORS.PYPYCD = PLANS.CTPLI3
		JOIN NKLB40P LINESOFBUSINESS 
		ON PLANS.CTLNBS = LINESOFBUSINESS.LBLOB
	--	JOIN NOHLHSPP FACILITIES 
	--	ON FACILITIES.HHHSPC = PLANS.CTHSPC
	WHERE PLANS.CTHSPC = @P_FACILITYCODE
		AND PLANS.CTPLID = @P_PLANCD;
				
		SELECT COUNT(*)
		INTO P_COUNT
		FROM NKPT36P FACINSURANCEOPTS
	--	JOIN NOHLHSPP FACILITIES 
	--	ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
		WHERE FACINSURANCEOPTS.PTHSPC = @P_FACILITYCODE
		AND FACINSURANCEOPTS.PTAPID = 'TC' 
		AND FACINSURANCEOPTS.PTTBID = 'PD';
		
		IF P_COUNT >0 THEN

			SELECT PTDATA
			INTO P_PTDATA 
			FROM NKPT36P FACINSURANCEOPTS
		--	JOIN NOHLHSPP FACILITIES 
		--	ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
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
