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

CREATE PROCEDURE NEXTACCOUNTNUMBERSEEDFOR (
    IN  P_HSP INTEGER,
    OUT O_ACCOUNTNUMBERSEED INTEGER,
    OUT O_MODTYPE CHAR(1) )

    DYNAMIC RESULT SETS 0
    LANGUAGE SQL
    SPECIFIC NXTACTSEED
    NOT DETERMINISTIC
    MODIFIES SQL DATA
    CALLED ON NULL INPUT
    SET OPTION DBGVIEW =*SOURCE

BEGIN ATOMIC

    -- Maximum account number value for facility
    DECLARE MAXACCOUNTNUMBERSEED INTEGER;
    -- Number used to reset the account number seed if we roll over the top
    DECLARE ACCTNUMSEEDRESTARTNUM INTEGER;    


    -- ========================================================================
    --  Step 1: Get the current account number values for the specified
    --          facility and lock the rows
    -- ========================================================================
    -- Get a cursor that will place a row-level lock on the number control file
    DECLARE ACCOUNTSEEDCURSOR CURSOR FOR
        SELECT 
            -- Next account number
            QCNAC#,
            -- Max account number
            QCMXA#,
            -- Use this if the account number values rolls over max
            QCSTA#
        FROM
            -- This is in HPDATA1
            HPADQCP2
        WHERE
            QCHSP# = P_HSP
        FOR UPDATE OF
            -- This should keep the row-level locks on both tables
            QCNAC#;
    
    -- Get a cursor that will place a row-level lock on the system control file
    DECLARE MODTYPECURSOR CURSOR FOR
        SELECT
            --- This is the MOD11 status of the account seed
            QCINSC
        FROM
            -- This is in HPDATA1
            HPADQCP1
        WHERE
            QCHSP# = P_HSP
        FOR UPDATE OF
            -- This should keep the row-level locks on both tables
            QCINSC;

    -- Open the cursors and lock the row
    OPEN ACCOUNTSEEDCURSOR;
    OPEN MODTYPECURSOR;

    -- Position the cursors and fetch the values
    FETCH
        ACCOUNTSEEDCURSOR
    INTO
        O_ACCOUNTNUMBERSEED,
        MAXACCOUNTNUMBERSEED,
        ACCTNUMSEEDRESTARTNUM;

    FETCH
        MODTYPECURSOR
    INTO
        O_MODTYPE;

        
    -- ========================================================================
    -- Step 2: Roll the account number seed OR increment it
    -- ========================================================================
    IF ( O_ACCOUNTNUMBERSEED >= MAXACCOUNTNUMBERSEED ) THEN

        -- Reset the acount number seed to the restart value
        UPDATE
            -- This is in HPDATA1
            HPADQCP2
        SET
            QCNAC# = ACCTNUMSEEDRESTARTNUM
        WHERE CURRENT OF
            ACCOUNTSEEDCURSOR;

        -- Set the MOD11 indicator for the facility
        UPDATE
            HPADQCP1
        SET
            -- The value is flipped to 'N' if 'Y' and vice-versa
            QCINSC = VALUE( NULLIF('Y',UPPER(O_MODTYPE) ), 'N' )
        WHERE CURRENT OF
            MODTYPECURSOR;

    ELSE

        -- Increment the account number seed using the cursor
        UPDATE
            -- This is in HPDATA1
            HPADQCP2
        SET
            QCNAC# = QCNAC# + 1
        WHERE CURRENT OF
            ACCOUNTSEEDCURSOR;

    END IF;

    CLOSE ACCOUNTSEEDCURSOR;
    CLOSE MODTYPECURSOR;
    
END;