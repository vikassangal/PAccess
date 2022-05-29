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

CREATE PROCEDURE FINDPATIENTSBYGUARANTORFOR ( 
    IN @P_FACILITYID INTEGER, 
    IN @P_FNAME VARCHAR(25), 
    IN @P_LNAME VARCHAR(25), 
    IN @P_SSN VARCHAR(11), 
    IN @P_GENDER VARCHAR(1)) 
    
    DYNAMIC RESULT SETS 1
    LANGUAGE SQL
    SPECIFIC FDPTSGUARF
    NOT DETERMINISTIC 
    MODIFIES SQL DATA
    CALLED ON NULL INPUT
    SET OPTION  ALWBLK = *ALLREAD, 
                ALWCPYDTA = *OPTIMIZE, 
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
    DECLARE SEARCHRESULTS CURSOR WITH RETURN FOR
    
        -- First, find the real names and associated aliases
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
            ''                                 AS MIDDLEINITIAL,                
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
            DEMOGRAPHICS.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS) 
              AND
            DEMOGRAPHICS.MDHSP# = @P_FACILITYID

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
            ''                                 AS DISPLAYMIDDLEINITIAL,            
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
            DEMOGRAPHICS.MDMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS) 
              AND
            ALIASES.AKMRC# IN (SELECT SELECTEDMRN FROM MRNUMBRS) 
              AND
            ALIASES.AKHSP# = @P_FACILITYID
        ORDER BY 
            DISPLAYLASTNAME, 
            DISPLAYFIRSTNAME, 
            DISPLAYMIDDLEINITIAL, 
            TYPEOFNAMEID;


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
            DEMOGRAPHICS.RWMRC# AS SELECTEDMRN
        FROM
            HXMRRWP DEMOGRAPHICS
        WHERE
            DEMOGRAPHICS.RWHSP# = @P_FACILITYID
               AND 
            DEMOGRAPHICS.RWACCT IN 
                ( SELECT DISTINCT 
                      GUARANTOR.MGGAR#
                  FROM
                      HPADMGP GUARANTOR
                  WHERE
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
                      GUARANTOR.MGGSEX = @P_GENDER ) )
    );

    -- ========================================================================
    --  Step 4: Open the cursor and leave it that way for the caller to use
    -- ========================================================================
    OPEN SEARCHRESULTS; 

END;