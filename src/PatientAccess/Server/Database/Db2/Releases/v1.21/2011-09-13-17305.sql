/**********************************************************************/
/* AS400 Short Name: CHKDUPLOC */
/* iSeries400 CheckForDupeLocations */
/* */
/* This proc makes a series of queries against the Room/Bed master */
/* joined to the Patient Physical (Patient Accounts) file and the */
/* Patient Demographics file. The purpose is to determine if a */
/* patient has already been assigned to a location. */
/* */
/* ************************************************************ */
/* * Perot Systems, Copyright 2003, All rights reserved(U.S.) * */
/* * * */
/* * This unpublished material is proprietary to Perot Sys. * */
/* * The methods and techniques described herein are * */
/* * considered trade secrets and/or confidential. * */
/* * Reproduction or distribution, in whole or in part, is * */
/* * forbidden except by express written permission of * */
/* * Perot Systems, Inc. * */
/* ************************************************************ */
/* */

SET PATH *LIBL ;

CREATE PROCEDURE CheckForDupeLocations (
  IN P_FACILITYID INTEGER ,
  IN P_LASTNAME VARCHAR(25) ,
  IN P_FIRSTNAME VARCHAR(15) ,
  IN P_ACCOUNTNUMBER INTEGER,
  IN P_SSN VARCHAR(11) ,
  IN P_DOB INTEGER ,
  IN P_MRN INTEGER ,
  IN P_ZIP VARCHAR(5) )

  DYNAMIC RESULT SETS 1
  LANGUAGE SQL
  SPECIFIC CHKDUPLOC
  NOT DETERMINISTIC
  MODIFIES SQL DATA
  CALLED ON NULL INPUT
  SET OPTION DBGVIEW =*SOURCE

P1 : BEGIN

  DECLARE ROW_COUNT INTEGER DEFAULT 0;

  DECLARE CURSOR1 CURSOR FOR
    SELECT *
    FROM SESSION.TEMP_RESULTS
    FOR READ ONLY
    OPTIMIZE FOR 100 ROWS ;

  DECLARE GLOBAL TEMPORARY TABLE SESSION.TEMP_RESULTS
  (
    STATUS INTEGER NOT NULL,
    ACCOUNTNUMBER INTEGER
  )
  WITH REPLACE
  ON COMMIT DELETE ROWS;

  IF P_MRN is not NULL AND P_MRN != 0 THEN
    -- select by MRN only (where not discharged)

    INSERT INTO SESSION.TEMP_RESULTS
    (
      SELECT DISTINCT 1, physical.lpacct
      from hpdata1.hpadlrp loc
      join hpdata1.hpadlpp physical
      on loc.lrhsp# = physical.lphsp#
      and loc.lracct = physical.lpacct
      join hpdata1.hpadmdp demo
      on physical.lphsp# = demo.mdhsp#
      and physical.lpmrc# = demo.mdmrc#
      where loc.LRHSP# = P_FacilityID
      and physical.LPMRC# = P_MRN
      and physical.LPLDD = 0
      and physical.lpacct != P_ACCOUNTNUMBER
    );

  END IF;

  SELECT count(*)
  INTO ROW_COUNT
  FROM SESSION.TEMP_RESULTS;

  IF ROW_COUNT = 0
    AND P_SSN is not NULL
    AND P_SSN != ''
    AND P_SSN not in ( '000000000', '000000001', '555555555', '777777777',
				'888888888' , '999999999' )
  THEN

    -- select by SSN only (where not discharged) SSN format: 123456789

    INSERT INTO SESSION.TEMP_RESULTS
    (
    SELECT DISTINCT 1, physical.lpacct
    from hpdata1.hpadlrp loc
    join hpdata1.hpadlpp physical
    on loc.lrhsp# = physical.lphsp#
    and loc.lracct = physical.lpacct
    join hpdata1.hpadmdp demo
    on lphsp# = mdhsp#
    and lpplnm = mdplnm
    and lppfnm = mdpfnm
    and lpmrc# = mdmrc#
    where loc.LRHSP# = P_FacilityID
    and demo.MDSSN9 = P_SSN
    and demo.MDSSN9 not in
    ( '000000000', '000000001', '555555555', '777777777', '888888888',
				'999999999' )
    and physical.LPLDD = 0
    and physical.lpacct != P_ACCOUNTNUMBER
    );

  END IF;

  SELECT count(*)
  INTO ROW_COUNT
  FROM SESSION.TEMP_RESULTS;

  IF ROW_COUNT = 0
    AND P_LASTNAME is not NULL
    AND P_LASTNAME != ''
    AND P_FIRSTNAME is not NULL
    AND P_FIRSTNAME != ''
    AND P_DOB is not NULL
    AND P_DOB != 0
    AND P_ZIP is not NULL
    AND P_ZIP != ''
  THEN
S1: BEGIN

    select count(*)
    into ROW_COUNT
    from hpdata1.hpadlrp loc
    join hpdata1.hpadlpp physical
    on loc.lrhsp# = physical.lphsp#
    and loc.lracct = physical.lpacct
    join hpdata1.hpadmdp demo
    on lphsp# = mdhsp#
    and lpplnm = mdplnm
    and lppfnm = mdpfnm
    and lpmrc# = mdmrc#
    where loc.LRHSP# = P_FacilityID
    and physical.LPPLNM = P_LastName
    and physical.LPPFNM = P_FirstName
    and demo.MDDOB8 = P_DOB -- (mmddyyyy int format)
    and demo.MDPZPA = P_ZIP
    and physical.LPLDD = 0
    and physical.lpacct != P_ACCOUNTNUMBER;

    IF ( ROW_COUNT != 0 ) THEN

        INSERT INTO SESSION.TEMP_RESULTS
        (
        SELECT DISTINCT 1, physical.lpacct
        from hpdata1.hpadlrp loc
        join hpdata1.hpadlpp physical
        on loc.lrhsp# = physical.lphsp#
        and loc.lracct = physical.lpacct
        join hpdata1.hpadmdp demo
        on lphsp# = mdhsp#
        and lpplnm = mdplnm
        and lppfnm = mdpfnm
        and lpmrc# = mdmrc#
        where loc.LRHSP# = P_FacilityID
        and physical.LPPLNM = P_LastName
        and physical.LPPFNM = P_FirstName
        and demo.MDDOB8 = P_DOB -- (mmddyyyy int format)
        and demo.MDPZPA = P_ZIP
        and physical.LPLDD = 0
        and physical.lpacct != P_ACCOUNTNUMBER
        );

        ELSE IF P_LASTNAME is not NULL
          AND P_LASTNAME != ''
          AND P_FIRSTNAME is not NULL
          AND P_FIRSTNAME != ''
          AND P_DOB is not NULL
          AND P_DOB != 0
        THEN

            INSERT INTO SESSION.TEMP_RESULTS
            (
            SELECT DISTINCT 2, physical.lpacct
            from hpdata1.hpadlrp loc
            join hpdata1.hpadlpp physical
            on loc.lrhsp# = physical.lphsp#
            and loc.lracct = physical.lpacct
            join hpdata1.hpadmdp demo
            on lphsp# = mdhsp#
            and lpplnm = mdplnm
            and lppfnm = mdpfnm
            and lpmrc# = mdmrc#
            where loc.LRHSP# = P_FacilityID
            and physical.LPPLNM = P_LastName
            and physical.LPPFNM = P_FirstName
            and demo.MDDOB8 = P_DOB -- (mmddyyyy int format)
            and physical.LPLDD = 0
            and physical.lpacct != P_ACCOUNTNUMBER
            );

        END IF;

        END IF;

    END S1;

    END IF;

    SELECT count(*)
    INTO ROW_COUNT
    FROM SESSION.TEMP_RESULTS;

    IF ROW_COUNT = 0
        AND P_LASTNAME is not NULL
        AND P_LASTNAME != ''
        AND P_FIRSTNAME is not NULL
        AND P_FIRSTNAME != ''
        AND P_DOB is not NULL
        AND P_DOB != 0
    THEN

        -- select where LastName, FirstName, DOB match (potential matches)

        INSERT INTO SESSION.TEMP_RESULTS
        (
        SELECT DISTINCT 2, physical.lpacct
        from hpdata1.hpadlrp loc
        join hpdata1.hpadlpp physical
        on loc.lrhsp# = physical.lphsp#
        and loc.lracct = physical.lpacct
        join hpdata1.hpadmdp demo
        on lphsp# = mdhsp#
        and lpplnm = mdplnm
        and lppfnm = mdpfnm
        and lpmrc# = mdmrc#
        where loc.LRHSP# = P_FacilityID
        and physical.LPPLNM = P_LastName
        and physical.LPPFNM = P_FirstName
        and demo.MDDOB8 = P_DOB -- (mmddyyyy int format)
        and physical.LPLDD = 0
        and physical.lpacct != P_ACCOUNTNUMBER
        );

        SELECT count(*) INTO ROW_COUNT
        FROM SESSION.TEMP_RESULTS;

        IF ROW_COUNT = 0
        THEN

        INSERT INTO SESSION.TEMP_RESULTS
        (
        SELECT 0, 0
        from sysibm.sysdummy1
        );

        END IF;

    END IF;

OPEN CURSOR1 ;

END P1 ;
