-------------------------------------------------------------------------------|
-- Perot Systems, Copyright 2008, All rights reserved(U.S.)
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

CREATE PROCEDURE GETALIASESFORPATIENT( 
    IN @P_FACILITYID INTEGER , 
    IN @P_MRN INTEGER) 

    DYNAMIC RESULT SETS 1 
    LANGUAGE SQL 
    SPECIFIC GETALIASPT
    NOT DETERMINISTIC 
    CALLED ON NULL INPUT 
    SET OPTION  ALWBLK = *ALLREAD , 
    ALWCPYDTA = *OPTIMIZE , 
    COMMIT = *NONE, 
    DBGVIEW = *SOURCE, 
    DECRESULT = (31, 31, 00), 
    DFTRDBCOL = *NONE, 
    DYNDFTCOL = *NO, 
    DYNUSRPRF = *USER, 
    SRTSEQ = *HEX 

BEGIN 

    DECLARE SETOFALIASES CURSOR FOR	

        SELECT 

            AKPLNM             AS LASTNAME,
            AKPFNM             AS FIRSTNAME,
            DEC6TODATE(AKEDAT) AS ENTRYDATE

        FROM 

           NMNHAKAP 

        WHERE 

            AKMRC# = @P_MRN
              AND
            AKHSP# = @P_FACILITYID

        ORDER BY

            ENTRYDATE,
            LASTNAME,
            FIRSTNAME;           

    OPEN SETOFALIASES;

END;