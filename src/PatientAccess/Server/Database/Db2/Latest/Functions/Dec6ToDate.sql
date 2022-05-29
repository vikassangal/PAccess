-------------------------------------------------------------------------------|
-- Perot Systems, Copyright 2008, All rights reserved(U.S.)
-------------------------------------------------------------------------------|
-- This unpublished material is proprietary to Perot Sys. The methods and 
-- techniques described herein are considered trade secrets and/or 
-- confidential. Reproduction or distribution, in whole or in part, is 
-- forbidden except by express written permission of Perot Systems, Inc.
-------------------------------------------------------------------------------|
-- NOTE: iSeries functions should not exceed 80 characters in width
-------------------------------------------------------------------------------|
---  0---|--- 10---|--- 20---|--- 30---|--- 40---|--- 50---|--- 60---|--- 70---|

SET PATH *LIBL;

CREATE FUNCTION DEC6TODATE( YYMMDD DECIMAL(6,0) ) 

    RETURNS DATE
    LANGUAGE SQL
    SPECIFIC DEC6TODATE
    DETERMINISTIC
    CALLED ON NULL INPUT
    SET OPTION DATFMT  = *YMD,
               DATSEP = '-'

BEGIN

   DECLARE CONVERTED DATE;
   
   -- We're fighting the *YMD format here. 1940/1/1 is the oldest
   -- date we can represent using this format.
   DECLARE EXIT HANDLER FOR SQLEXCEPTION RETURN DATE( '40-01-01' );
   
   SET CONVERTED = 
     DATE( INSERT( INSERT( DIGITS( YYMMDD ),3,0,'-') ,6,0,'-') );
   
   RETURN COALESCE( CONVERTED, DATE( '40-01-01' ) );
   
END;