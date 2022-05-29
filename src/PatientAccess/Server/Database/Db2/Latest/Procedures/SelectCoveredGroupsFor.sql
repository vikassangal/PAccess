/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTCOVEREDGROUPSFOR - SQL PROC FOR PX	      */        
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
/* 02/08/2008 I  Sophie Zhang      I  Remove NOHLHSPP (cr comments)   */
/* 02/08/2008 I	 Sophie Zhang      I  Left join FacInsuranceOpts (cr) */        
/*************I********************I***********************************/        
                                                                                                                   
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTCOVEREDGROUPSFOR( 
		IN @P_PLANCD VARCHAR(5),
		IN @P_EFFECTIVEDATE DECIMAL(8,0),
		IN @P_APPROVALDATE DECIMAL(8,0),
        IN @P_FACILITYID INTEGER ,
        IN @P_ADMITDATE DECIMAL(8,0) )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELCVGRFOR                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN
		DECLARE P_COUNT INTEGER DEFAULT 0;
		DECLARE P_PTDATA VARCHAR(74) DEFAULT '';
		DECLARE P_TERMGRACEDAYS INTEGER DEFAULT 0;
		DECLARE P_CANXGRACEDAYS INTEGER DEFAULT 0;				
        
		DECLARE CURSOR1 CURSOR FOR                  
                SELECT DISTINCT
                0 AS COVEREDGROUPID,
                0 AS PLANID,
			TRIM(COVEREDGROUPS.GCGCOV) AS GROUPCOVERED,
			0 AS PHONENUMBERID,
			TRIM(COVEREDGROUPS.GCGCPH) AS PHONENUMBER,
			0 AS ADDRESSID,
			TRIM(COVEREDGROUPS.GCGCA1) AS ADDRESS1,
			TRIM(COVEREDGROUPS.GCGCA2) AS ADDRESS2,
			TRIM(COVEREDGROUPS.GCGCCI) AS CITY,
			0 AS COUNTYID,
			COVEREDGROUPS.GCGCST AS STATECODE,
			TRIM(COVEREDGROUPS.GCGCZP) AS POSTALCODE,
			'USA' AS COUNTRYCODE,
			1 AS TYPEOFADDRESSID,
			'P' AS ADDRESSDESCRIPTION,
			0 AS EMPLOYERID,
			TRIM(EMPLOYERS.EMNAME) AS NAME,
			TRIM(EMPLOYERS.EMNEID) AS NATIONALID,
			EMPLOYERS.EMCODE AS EMPLOYERCODE
			FROM NKGC08P AS COVEREDGROUPS
			
			JOIN NKCT01P AS PLANS 
			ON COVEREDGROUPS.GCHSPC = PLANS.CTHSPC
			AND COVEREDGROUPS.GCPLID = PLANS.CTPLID
			AND COVEREDGROUPS.GCCBGD = PLANS.CTCBGD
			AND COVEREDGROUPS.GCPLAD = PLANS.CTPLAD
			
			JOIN NKPY25P AS PAYORS 
			ON  PAYORS.PYHSPC = PLANS.CTHSPC 
			AND PAYORS.PYPYCD = PLANS.CTPLI3
			
			JOIN FF0015P AS FOLLOWUPUNITS 
			ON FOLLOWUPUNITS.FFHSPC = PLANS.CTHSPC
			
			LEFT JOIN NCEM10P EMPLOYERS 
			ON EMPLOYERS.EMFUUN = FOLLOWUPUNITS.FFFUUN 
			AND SUBSTRING(COVEREDGROUPS.GCGCOV, 1, 23) = EMPLOYERS.EMNAME
			AND COVEREDGROUPS.GCGCCD = EMPLOYERS.EMCODE * 100000
			
			WHERE FOLLOWUPUNITS.FFHSPN = @P_FACILITYID
			AND PLANS.CTPLID = @P_PLANCD
			AND PLANS.CTPLAD = @P_APPROVALDATE
			AND PLANS.CTCBGD = @P_EFFECTIVEDATE
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
				);
			
			SELECT COUNT(*)
			INTO P_COUNT
			FROM NKPT36P FACINSURANCEOPTS
			JOIN NOHLHSPP FACILITIES 
			ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
			WHERE FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD' 
			AND FACILITIES.HHHSPN = @P_FACILITYID;
			
			IF P_COUNT >0 THEN

			SELECT PTDATA
			INTO P_PTDATA 
			FROM NKPT36P FACINSURANCEOPTS
			
			JOIN NOHLHSPP FACILITIES 
			ON FACILITIES.HHHSPC = FACINSURANCEOPTS.PTHSPC
			
			WHERE FACINSURANCEOPTS.PTAPID = 'TC' 
			AND  FACINSURANCEOPTS.PTTBID = 'PD' 
			AND FACILITIES.HHHSPN = @P_FACILITYID
			FETCH FIRST ROW ONLY;
			
			SET P_TERMGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 7,3));
			SET P_CANXGRACEDAYS = INTEGER(SUBSTR(P_PTDATA, 10,3));
			
			END IF;
				
                OPEN CURSOR1 ; 
                                                                 
                END P1  ;                                                       
 