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
  
CREATE PROCEDURE FINDPATIENTSFOR(
	IN @P_FACILITYID INTEGER,
    IN @P_MRN INTEGER,
	IN @P_ACCOUNTNUMBER INTEGER,
    IN @P_SSN CHAR(9),
	IN @P_FNAME VARCHAR(25), 
	IN @P_LNAME VARCHAR(25), 
	IN @P_GENDER VARCHAR(1), 
	IN @P_MONTH INTEGER, 
	IN @P_YEAR INTEGER) 
    
	DYNAMIC RESULT SETS 1
	LANGUAGE SQL
	SPECIFIC FINDPTSFOR
	NOT DETERMINISTIC
	READS SQL DATA
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

    DECLARE @V_MEDICALRECORDNUMBER INTEGER;
    DECLARE @V_QUERYTEMPLATE VARCHAR(8192);
    DECLARE @V_DEMOFILTER VARCHAR(2048) DEFAULT ' ';
    DECLARE @V_ALIASFILTER VARCHAR(2048) DEFAULT ' ';
    DECLARE SEARCHCURSOR CURSOR WITH RETURN FOR SEARCHSTATEMENT;
    
    SET @V_QUERYTEMPLATE = 
        '-- First, find the real names and associated aliases
        SELECT DISTINCT    
            DEMOGRAPHICS.MDSSN9                AS SSN,
            DEMOGRAPHICS.MDSEX                 AS SEXCODE,
            DEMOGRAPHICS.MDMRC#                AS MRN,
            DEMOGRAPHICS.MDDOB8                AS DOB,
            1                                  AS TYPEOFNAMEID,
            TRIM ( DEMOGRAPHICS.MDPFNM )       AS DISPLAYFIRSTNAME,
            TRIM ( DEMOGRAPHICS.MDPLNM )       AS DISPLAYLASTNAME,
            TRIM ( DEMOGRAPHICS.MDPMI )        AS DISPLAYMIDDLEINITIAL,  
            DEMOGRAPHICS.MDNMTL				   AS TITLE,        
            TRIM ( ALIASES.AKPFNM )            AS FIRSTNAME,
            TRIM ( ALIASES.AKPLNM )            AS LASTNAME,        
            ''''                               AS MIDDLEINITIAL,                
            DEMOGRAPHICS.MDMADR                AS PHYSICALSTREET,
            DEMOGRAPHICS.MDMCIT                AS PHYSICALCITY,
            DEMOGRAPHICS.MDMSTE                AS PHYSICALSTATE,
            RTRIM ( DEMOGRAPHICS.MDMZPA ) ||
            RTRIM ( DEMOGRAPHICS.MDMZ4A )      AS PHYSICALZIP,
            DEMOGRAPHICS.MDPADR                AS MAILINGSTREET,
            DEMOGRAPHICS.MDPCIT                AS MAILINGCITY,
            DEMOGRAPHICS.MDPSTE                AS MAILINGSTATE,
            RTRIM( DEMOGRAPHICS.MDPZPA ) ||
            RTRIM( DEMOGRAPHICS.MDPZ4A )       AS MAILINGZIP
        FROM     
            HPADMDP DEMOGRAPHICS
        LEFT OUTER JOIN     
            NMNHAKAP ALIASES        
        ON 
            DEMOGRAPHICS.MDHSP# = ALIASES.AKHSP#
              AND 
            DEMOGRAPHICS.MDMRC# = ALIASES.AKMRC#        
        WHERE
            {1}

    	UNION

        -- Now find the aliases
        SELECT DISTINCT    
            DEMOGRAPHICS.MDSSN9                AS SSN,
            DEMOGRAPHICS.MDSEX                 AS SEXCODE,
            DEMOGRAPHICS.MDMRC#                AS MRN,
            DEMOGRAPHICS.MDDOB8                AS DOB,
            2                                  AS TYPEOFNAMEID,
            TRIM ( ALIASES.AKPFNM )            AS DISPLAYFIRSTNAME,
            TRIM ( ALIASES.AKPLNM )            AS DISPLAYLASTNAME,
            ''''                               AS DISPLAYMIDDLEINITIAL,
            DEMOGRAPHICS.MDNMTL				   AS TITLE,                  
            TRIM ( DEMOGRAPHICS.MDPFNM )       AS FIRSTNAME,
            TRIM ( DEMOGRAPHICS.MDPLNM )       AS LASTNAME,
            TRIM ( DEMOGRAPHICS.MDPMI )        AS MIDDLEINITIAL,
            DEMOGRAPHICS.MDMADR                AS PHYSICALSTREET,
            DEMOGRAPHICS.MDMCIT                AS PHYSICALCITY,
            DEMOGRAPHICS.MDMSTE                AS PHYSICALSTATE,
            RTRIM ( DEMOGRAPHICS.MDMZPA ) ||
            RTRIM ( DEMOGRAPHICS.MDMZ4A )      AS PHYSICALZIP,
            DEMOGRAPHICS.MDPADR                AS MAILINGSTREET,
            DEMOGRAPHICS.MDPCIT                AS MAILINGCITY,
            DEMOGRAPHICS.MDPSTE                AS MAILINGSTATE,
            RTRIM ( DEMOGRAPHICS.MDPZPA ) ||
            RTRIM ( DEMOGRAPHICS.MDPZ4A )      AS MAILINGZIP
        FROM     
            HPADMDP DEMOGRAPHICS
        LEFT OUTER JOIN     
            NMNHAKAP ALIASES        
        ON 
            DEMOGRAPHICS.MDHSP# = ALIASES.AKHSP#
              AND 
            DEMOGRAPHICS.MDMRC# = ALIASES.AKMRC#
        WHERE
            {2}
        ORDER BY 
            DISPLAYLASTNAME, 
            DISPLAYFIRSTNAME, 
            DISPLAYMIDDLEINITIAL, 
            TYPEOFNAMEID';

    -- If we are given a MRN, then just use that        
    IF @P_MRN IS NOT NULL THEN
              
        SET @V_MEDICALRECORDNUMBER = @P_MRN;
    
    -- If we are given an account number, find an MRN
    ELSEIF @P_ACCOUNTNUMBER IS NOT NULL THEN
    
        -- Look in the accounts table first
        SET @V_MEDICALRECORDNUMBER = 
            (SELECT DISTINCT 
                LPMRC# 
             FROM 
                HPADLPP 
             WHERE 
                LPACCT = @P_ACCOUNTNUMBER 
                  AND 
                LPHSP# = @P_FACILITYID);

        -- If the patient only has a demographic record,
        -- see if we can find an account number in that
        IF @V_MEDICALRECORDNUMBER IS NULL THEN

            SET @V_MEDICALRECORDNUMBER = 
                (SELECT DISTINCT 
                    MDMRC# 
                 FROM 
                    HPADMDP
                 WHERE 
                    MDACCT = @P_ACCOUNTNUMBER 
                      AND 
                    MDHSP# = @P_FACILITYID);

        END IF;
        
        -- Didn't find a matching MRN
        IF @V_MEDICALRECORDNUMBER IS NULL THEN
        
			RETURN 0;
        
        END IF;
        
    END IF;            
            
    IF @V_MEDICALRECORDNUMBER IS NULL AND @P_SSN IS NOT NULL THEN

        SET @V_DEMOFILTER = 
          ' ( DEMOGRAPHICS.MDHSP# = ' || 
          @P_FACILITYID || 
          ' AND DEMOGRAPHICS.MDMRC# IN ' ||
          '( SELECT MDMRC# FROM HPADMDP WHERE MDSSN9 = ''' || 
          @P_SSN || 
          ''' AND MDHSP# = ' || 
          @P_FACILITYID ||
          ' ) ) ';

        SET @V_ALIASFILTER = 
          ' ( ALIASES.AKHSP# = ' || 
          @P_FACILITYID || 
          ' AND ALIASES.AKMRC# IN ' ||
          '( SELECT MDMRC# FROM HPADMDP WHERE MDSSN9 = ''' || 
          @P_SSN || 
          ''' AND MDHSP# = ' || 
          @P_FACILITYID ||
          ' ) ) ';
        
    ELSEIF @V_MEDICALRECORDNUMBER IS NOT NULL THEN 
    
        SET @V_DEMOFILTER = 
          ' ( DEMOGRAPHICS.MDHSP# = ' || 
          @P_FACILITYID || 
          ' AND DEMOGRAPHICS.MDMRC# = ' || 
          @V_MEDICALRECORDNUMBER || 
          ' ) ';    

        SET @V_ALIASFILTER = 
          ' ( DEMOGRAPHICS.MDHSP# = ' || 
          @P_FACILITYID || 
          ' AND DEMOGRAPHICS.MDMRC# = ' || 
          @V_MEDICALRECORDNUMBER || 
          ' ) AND ( ALIASES.AKHSP# = ' || 
          @P_FACILITYID || 
          ' AND ALIASES.AKMRC# = ' || 
          @V_MEDICALRECORDNUMBER || 
          ' ) ';
            
    ELSE
    
        SET @V_DEMOFILTER = 
          ' ( DEMOGRAPHICS.MDHSP# = ' || 
          @P_FACILITYID || 
          ' ) ';    

        SET @V_ALIASFILTER = 
          ' ( DEMOGRAPHICS.MDHSP# = ' || 
          @P_FACILITYID || 
          ' ) ';    
    
    END IF;
       
    -- Add last name filter
    IF @P_LNAME IS NOT NULL THEN
        
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( DEMOGRAPHICS.MDPLSN LIKE (''' || 
          UPPER(@P_LNAME) || 
          '%'') ) ';
        
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( ALIASES.AKPLSN LIKE (''' || 
          UPPER(@P_LNAME) || 
          '%'') ) ';
          
    END IF;
            
    -- Add first name filter
    IF @P_FNAME IS NOT NULL THEN
        
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( DEMOGRAPHICS.MDPFSN LIKE (''' || 
          UPPER(@P_FNAME) || 
          '%'') ) ';
        
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( ALIASES.AKPFSN LIKE (''' || 
          UPPER(@P_FNAME) || 
          '%'') ) ';
          
    END IF;

    -- add gender filter
    IF @P_GENDER IS NOT NULL THEN
    
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( DEMOGRAPHICS.MDSEX = ''' || 
          UPPER(@P_GENDER) || 
          ''') ';
          
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( DEMOGRAPHICS.MDSEX = ''' || 
          UPPER(@P_GENDER) || 
          ''') ';
        
    END IF;

    -- add date-of-birth filter
    IF @P_MONTH IS NOT NULL AND @P_YEAR IS NOT NULL THEN
    
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) BETWEEN (DATE(''' || 
          @P_MONTH || 
          '/01/' || 
          @P_YEAR || 
          ''') - 2 YEARS) AND LAST_DAY((DATE(''' || 
          @P_MONTH || 
          '/01/' || 
          @P_YEAR || 
          ''') + 2 YEARS)) ) AND ' ||
          '( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' || 
          @P_MONTH || ')';
          
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) BETWEEN (DATE(''' || 
          @P_MONTH || 
          '/01/' || 
          @P_YEAR || 
          ''') - 2 YEARS) AND LAST_DAY((DATE(''' || 
          @P_MONTH || 
          '/01/' || 
          @P_YEAR || 
          ''') + 2 YEARS)) ) AND ' ||
          '( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' || 
          @P_MONTH || ')';
        
    ELSEIF @P_YEAR IS NOT NULL THEN
    
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) BETWEEN (DATE(''01/01/' || 
          @P_YEAR || 
          ''') - 2 YEARS) AND (DATE(''12/31/' || 
          @P_YEAR || 
          ''') + 2 YEARS ) ) ';
          
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) BETWEEN (DATE(''01/01/' || 
          @P_YEAR || 
          ''') - 2 YEARS) AND (DATE(''12/31/' || 
          @P_YEAR || 
          ''') + 2 YEARS ) ) ';
        
    ELSEIF @P_MONTH IS NOT NULL THEN
    
        SET @V_DEMOFILTER = 
          @V_DEMOFILTER || 
          ' AND ( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' || 
          @P_MONTH || 
          ') ';
          
        SET @V_ALIASFILTER = 
          @V_ALIASFILTER || 
          ' AND ( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' || 
          @P_MONTH || 
          ') ';
          
    END IF;
    
    -- Replace the placeholders with the filter strings
    SET @V_QUERYTEMPLATE = REPLACE( @V_QUERYTEMPLATE, '{1}', @V_DEMOFILTER );
    SET @V_QUERYTEMPLATE = REPLACE( @V_QUERYTEMPLATE, '{2}', @V_ALIASFILTER );
    
    PREPARE SEARCHSTATEMENT FROM @V_QUERYTEMPLATE;
    
    -- Go get the result set
    OPEN SEARCHCURSOR;

END;
