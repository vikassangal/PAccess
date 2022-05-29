-------------------------------------------------------------------------------|
-- Perot Systems, Copyright 2003, All rights reserved(U.S.)
-------------------------------------------------------------------------------|
-- This unpublished material is proprietary to Perot Sys. The methods and 
-- techniques described herein are considered trade secrets and/or 
-- confidential. Reproduction or distribution, in whole or in part, is 
-- forbidden except by express written permission of Perot Systems, Inc.
-------------------------------------------------------------------------------|
-- NOTE: iSeries stored procs should not exceed 80 characters in width
-------------------------------------------------------------------------------|
---  0---|--- 10---|--- 20---|--- 30---|--- 40---|--- 50---|--- 60---|--- 70---|

SET PATH *LIBL; 
  
CREATE PROCEDURE PATIENTSMATCHINGGUARANTOR ( 
	IN @P_FACILITYID INTEGER , 
	IN @P_FNAME VARCHAR(25) , 
	IN @P_LNAME VARCHAR(25) , 
	IN @P_SSN VARCHAR(11) , 
	IN @P_GENDER VARCHAR(1) , 
	IN @P_HSPCODE VARCHAR(3) ) 
	
	DYNAMIC RESULT SETS 1
	LANGUAGE SQL
	SPECIFIC PATMTCHGAR
	NOT DETERMINISTIC 
	MODIFIES SQL DATA
	CALLED ON NULL INPUT
	SET OPTION  ALWBLK = *ALLREAD, 
				ALWCPYDTA = *OPTIMIZE, 
				COMMIT = *NONE, 
				DBGVIEW = *SOURCE, 
				DECRESULT = (31, 31, 00), 
				DFTRDBCOL = *NONE, 
				DYNDFTCOL = *NO, 
				DYNUSRPRF = *USER, 
				SRTSEQ = *HEX
	   
BEGIN

    -- ========================================================================
    --  Step 1: Define the search results cursor which we will return to the
    --          caller
    -- ========================================================================
	DECLARE SEARCHRESULTS CURSOR FOR		 
	
		--PATIENTS WITH CONTACT POINT TYPE AS 'MAILING' 			
		SELECT DISTINCT 
			DEM.MDHSP#				AS FACILITYID,
			0						AS PERSONID,
			AKA.AKEDAT				AS ENTRYDATE,
			DEM.MDSSN9				AS SSN,
			@P_HSPCODE				AS HSPCODE,
			DEM.MDSEX				AS SEXCODE,
			DEM.MDMRC#				AS MRN, 
			DEM.MDDOB8				AS DOB,
			2						AS TYPEOFNAMEID,
			TRIM( AKA.AKPFNM )		AS FIRSTNAME,
			TRIM( AKA.AKPLNM )		AS LASTNAME,
			''						AS MIDDLEINITIAL,				 
			1						AS DISPLAYTYPEOFNAMEID,
			DEM.MDPFNM				AS DISPLAYFIRSTNAME,
			DEM.MDPLNM				AS DISPLAYLASTNAME,
			DEM.MDPMI				AS DISPLAYMIDDLEINITIAL,
			DEM.MDNMTL				AS TITLE,
			''						AS CONFIDENTIALFLAG,
			6						AS CONTACTPOINTTYPEID,
			'MAILING'				AS TYPEOFCONTACTPOINT,
			DEM.MDMADR				AS ADDRESS1,
			''						AS ADDRESS2,
			DEM.MDMCIT				AS CITY, 
			RTRIM( DEM.MDMZPA ) || 
			RTRIM ( DEM.MDMZ4A )	AS POSTALCODE, 
			0						AS STATEID, 
			DEM.MDMSTE				AS STATECODE, 
			''						AS STATEDESCRIPTION, 
			0						AS COUNTRYID, 
			''						AS COUNTRYCODE, 
			''						AS COUNTRYDESCRIPTION 
		FROM 
			HPADMDP DEM
		LEFT OUTER JOIN 
			NMNHAKAP AKA 
		ON 
			AKA.AKHSP# = @P_FACILITYID
			  AND
			DEM.MDHSP# = AKA.AKHSP# 
			  AND 
			DEM.MDMRC# = AKA.AKMRC#
		WHERE 
			DEM.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS)
			  AND
			DEM.MDHSP# = @P_FACILITYID
	UNION 
		
		--PATIENTS WITH CONTACT POINT TYPE AS 'PHYSICAL'
		SELECT DISTINCT 
			DEM.MDHSP#				AS FACILITYID,
			0						AS PERSONID,
			AKA.AKEDAT				AS ENTRYDATE,
			DEM.MDSSN9				AS SSN,
			@P_HSPCODE				AS HSPCODE,
			DEM.MDSEX				AS SEXCODE,
			DEM.MDMRC#				AS MRN, 
			DEM.MDDOB8				AS DOB,
			2						AS TYPEOFNAMEID,
			TRIM( AKA.AKPFNM )		AS FIRSTNAME,
			TRIM( AKA.AKPLNM )		AS LASTNAME,
			''						AS MIDDLEINITIAL,				 
			1						AS DISPLAYTYPEOFNAMEID,
			DEM.MDPFNM				AS DISPLAYFIRSTNAME,
			DEM.MDPLNM				AS DISPLAYLASTNAME,
			DEM.MDPMI				AS DISPLAYMIDDLEINITIAL,
			DEM.MDNMTL				AS TITLE,
			''						AS CONFIDENTIALFLAG,
			0						AS CONTACTPOINTTYPEID,	 
			'PHYSICAL'				AS TYPEOFCONTACTPOINT, 
			DEM.MDPADR				AS ADDRESS1, 
			''						AS ADDRESS2,
			DEM.MDPCIT				AS CITY, 
			RTRIM( DEM.MDPZPA ) || 
			RTRIM ( DEM.MDPZ4A )	AS POSTALCODE, 
			0						AS STATEID, 
			DEM.MDPSTE				AS STATECODE, 
			''						AS STATEDESCRIPTION, 
			0						AS COUNTRYID, 
			''						AS COUNTRYCODE, 
			''						AS COUNTRYDESCRIPTION 
		FROM 
			HPADMDP DEM
		LEFT OUTER JOIN 
			NMNHAKAP AKA 
		ON 
			AKA.AKHSP# = @P_FACILITYID
			  AND	
			DEM.MDHSP# = AKA.AKHSP# 
			  AND 
			DEM.MDMRC# = AKA.AKMRC#
		WHERE 
			DEM.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS)
			  AND
			DEM.MDHSP# = @P_FACILITYID

	UNION 
	  
	--ALIASES WITH CONTACT POINT TYPE AS 'MAILING' 
	SELECT	DISTINCT 
		DEM.MDHSP#					AS FACILITYID, 
		0							AS PERSONID, 
		AKA.AKEDAT					AS ENTRYDATE,	 
		DEM.MDSSN9					AS SSN, 
		@P_HSPCODE					AS HSPCODE, 
		DEM.MDSEX					AS SEXCODE, 
		DEM.MDMRC#					AS MRN, 
		DEM.MDDOB8					AS DOB, 
		1							AS TYPEOFNAMEID, 		
		DEM.MDPFNM					AS FIRSTNAME,
		DEM.MDPLNM					AS LASTNAME,
		''							AS MIDDLEINITIAL, 
		2							AS DISPLAYTYPEOFNAMEID, 
		TRIM( AKA.AKPFNM )			AS DISPLAYFIRSTNAME, 
		TRIM( AKA.AKPLNM )			AS DISPLAYLASTNAME, 
		''							AS DISPLAYMIDDLEINITIAL,
		DEM.MDNMTL					AS TITLE, 
		''							AS CONFIDENTIALFLAG,
		6							AS CONTACTPOINTTYPEID,
		'MAILING'					AS TYPEOFCONTACTPOINT, 
		DEM.MDMADR					AS ADDRESS1, 
		''							AS ADDRESS2,
		DEM.MDMCIT					AS CITY, 
		RTRIM( DEM.MDMZPA ) || 
		RTRIM ( DEM.MDMZ4A )		AS POSTALCODE, 
		0							AS STATEID, 
		DEM.MDMSTE					AS STATECODE, 
		''							AS STATEDESCRIPTION, 
		0							AS COUNTRTID, 
		''							AS COUNTRYCODE, 
		''							AS COUNTRYDESCRIPTION 
	FROM 
		NMNHAKAP AKA
	INNER JOIN 
		HPADMDP DEM 
	ON 
		AKA.AKHSP# = @P_FACILITYID
		  AND
		DEM.MDHSP# = AKA.AKHSP# 
		  AND 
		DEM.MDMRC# = AKA.AKMRC#
	WHERE 
		DEM.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS) 
		  AND
		DEM.MDHSP# = @P_FACILITYID
				 
	UNION 
	
	--ALIASES WITH CONTACT POINT TYPE AS 'PHYSICAL'
	SELECT	DISTINCT 
		DEM.MDHSP#					AS FACILITYID, 
		0							AS PERSONID, 
		AKA.AKEDAT					AS ENTRYDATE,	 
		DEM.MDSSN9					AS SSN, 
		@P_HSPCODE					AS HSPCODE, 
		DEM.MDSEX					AS SEXCODE, 
		DEM.MDMRC#					AS MRN, 
		DEM.MDDOB8					AS DOB, 
		1							AS TYPEOFNAMEID, 
		DEM.MDPFNM					AS FIRSTNAME,
		DEM.MDPLNM					AS LASTNAME,
		''							AS MIDDLEINITIAL, 
		2							AS DISPLAYTYPEOFNAMEID, 
		TRIM( AKA.AKPFNM )			AS DISPLAYFIRSTNAME, 
		TRIM( AKA.AKPLNM )			AS DISPLAYLASTNAME, 
		''							AS DISPLAYMIDDLEINITIAL,
		DEM.MDNMTL					AS TITLE, 
		''							AS CONFIDENTIALFLAG,
		0							AS CONTACTPOINTTYPEID,
		'PHYSICAL'					AS TYPEOFCONTACTPOINT, 
		DEM.MDPADR					AS ADDRESS1, 
		''							AS ADDRESS2,
		DEM.MDPCIT					AS CITY, 
		RTRIM( DEM.MDPZPA ) || 
		RTRIM ( DEM.MDPZ4A )		AS POSTALCODE, 
		0							AS STATEID, 
		DEM.MDPSTE					AS STATECODE, 
		''							AS STATEDESCRIPTION, 
		0							AS COUNTRTID, 
		''							AS COUNTRYCODE, 
		''							AS COUNTRYDESCRIPTION 
	FROM 
		NMNHAKAP AKA	
	INNER JOIN 
		HPADMDP DEM 
	ON 
		AKA.AKHSP# = @P_FACILITYID
		  AND
		DEM.MDHSP# = AKA.AKHSP# 
		  AND 
		DEM.MDMRC# = AKA.AKMRC#
	WHERE 
		DEM.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS) 
		  AND
		DEM.MDHSP# = @P_FACILITYID

	ORDER BY 
		MRN, 
		DISPLAYTYPEOFNAMEID ASC, 
		LASTNAME , 
		FIRSTNAME , 
		MIDDLEINITIAL , 
		ENTRYDATE 
	DESC 
	FOR	READ ONLY 
	OPTIMIZE FOR 10 ROWS;


    -- ========================================================================
    --  Step 2: Define a temporary table to hold the medical record numbers of
    --          all the patients related to the matching guarantors
    -- ========================================================================
	DECLARE GLOBAL TEMPORARY TABLE MRNUMBRS
	(
		SELECTEDMRN DECIMAL(9,0)
	)
	WITH REPLACE;


    -- ========================================================================
    --  Step 3: Load the temp table using the search parameters
    -- ========================================================================
	INSERT INTO 
		MRNUMBRS 
	(
		SELECT
			PAT.RWMRC# AS SELECTEDMRN
		FROM
			HXMRRWP PAT,
			HPADMGP GUARANTOR
		WHERE
			PAT.RWHSP# = @P_FACILITYID
			  AND
			GUARANTOR.MGHSP# = @P_FACILITYID
			  AND
			( @P_FNAME IS NULL
			   OR 
			  GUARANTOR.MGGFSN LIKE UPPER( @P_FNAME ) || '%' ) 
			  AND		
			( @P_LNAME IS NULL			 
				OR 
			  GUARANTOR.MGGLSN LIKE UPPER( @P_LNAME ) || '%' )						 
			  AND		
			( @P_SSN IS NULL
				OR 
			  GUARANTOR.MGGSSN = @P_SSN ) 
			  AND		
			( @P_GENDER IS NULL			 
				OR 
			  GUARANTOR.MGGSEX = @P_GENDER )
			  AND
			GUARANTOR.MGHSP# = PAT.RWHSP# 
			  AND 
			PAT.RWACCT = GUARANTOR.MGGAR#
	);

    -- ========================================================================
    --  Step 4: Open the cursor and leave it that way for the caller to use
    -- ========================================================================
	OPEN SEARCHRESULTS; 

END;
