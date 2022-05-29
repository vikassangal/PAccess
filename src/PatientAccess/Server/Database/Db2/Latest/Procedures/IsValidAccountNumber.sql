-------------------------------------------------------------------------------|
-- Perot Systems, Copyright 2009, All rights reserved(U.S.)
-------------------------------------------------------------------------------|
-- This unpublished material is proprietary to Perot Sys. The methods and
-- techniques described herein are considered trade secrets and/or
-- confidential. Reproduction or distribution, in whole or in part, is
-- forbidden except by express written permission of Perot Systems, Inc.
-------------------------------------------------------------------------------|
-- NOTE: iSeries stored procs should not exceed 80 characters in width
-------------------------------------------------------------------------------|
---  0---|--- 10---|--- 20---|--- 30---|--- 40---|--- 50---|--- 60---|--- 70---|
CREATE PROCEDURE ISVALIDACCOUNTNUMBER (
    IN P_HSP INTEGER ,
    IN P_HSPCODE CHAR(3) ,
    IN P_ACCOUNTNUMBER INTEGER ,
    OUT O_ISVALIDACCTNBR CHAR(1) )
        
    DYNAMIC RESULT SETS 0
    LANGUAGE SQL
    SPECIFIC ISVALACCTN
    NOT DETERMINISTIC
    READS SQL DATA
    CALLED ON NULL INPUT
    SET OPTION DBGVIEW =*SOURCE
    
BEGIN

	-- If we fail miserably, tell the client the number is no good.
	-- This can happen if re run into non-numeric values in the
	-- millenia field and for other data-conversion errors. In that
	-- case, we will burn the account number we generated becuase
	-- of the level of uncertaintly about its state.
	DECLARE EXIT HANDLER FOR SQLEXCEPTION SET O_ISVALIDACCTNBR = 0;
	
    -- Is there a current account in the facility with this account
    -- number? If so, set our result variable to zero (false)
    IF EXISTS 
    ( 
    
        SELECT 
          LPACCT 
        FROM 
          HPADLPP -- HPDATA1 Library 
        WHERE 
          LPHSP# = P_HSP 
            AND 
          LPACCT = P_ACCOUNTNUMBER 
            
    ) THEN SET O_ISVALIDACCTNBR = 0;

    
    -- Is there an entry in the MPI table with this account number?
    -- If so, set our result variable to zero (false)
    ELSEIF EXISTS 
    ( 
    
        SELECT 
          RWACCT 
        FROM 
          HXMRRWP -- HPDATA1 Library
        WHERE 
          RWHSP# = P_HSP 
            AND 
          RWACCT = P_ACCOUNTNUMBER
    
    ) THEN SET O_ISVALIDACCTNBR = 0;

    
    -- Is there an entry in the FUS Master with this account number?
    -- If so, set our result variable to zero (false)
    ELSEIF EXISTS 
    ( 
    
        SELECT 
          PMPT#9 
        FROM 
          PM0001P -- PADATA Library
        WHERE 
          PMHSPC = P_HSPCODE 
            AND 
          PMPT#9 = P_ACCOUNTNUMBER 
     
    ) THEN SET O_ISVALIDACCTNBR = 0;

    
    -- Is there a purged account, less than ten years old, with this
    -- account number? If so, set our result variable to zero (false)
    ELSEIF EXISTS 
    ( 
    
        SELECT 
          PMPT#9
        FROM 
          PMP002P -- PADATA Library
        WHERE 
          PMHSPC = P_HSPCODE
            AND 
          PMPT#9 = P_ACCOUNTNUMBER
            AND            
          (
            -- Current date must be outside the purgedate + 10yr window
            -- We use the greater of the ADT and FUS dates as the value.
            -- If the conversion fails, the function will return a default
            -- value that is the maximum value for a date field so that
            -- the number will be evaluated as invalid. This may not be
            -- correct, but it is the safest assumption given bad data
              ( MAX( DEC8TODATE( DECIMAL( PMFPDC || DIGITS(PMFPDT), 8, 0 ), 
                                 DATE('12/31/9989') ), 
                     DEC8TODATE( DECIMAL( PMAPDC || DIGITS(PMAPDT), 8, 0 ), 
                                 DATE('12/31/9989') ) ) + 10 YEARS ) 
               >= CURRENT_DATE
           )
            
    ) THEN SET O_ISVALIDACCTNBR = 0;

    
    -- If we got here, then it must be a valid account number
    ELSE
    
        SET O_ISVALIDACCTNBR = 1;
        
    END IF;

END;