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

CREATE FUNCTION DEC8TODATE( YYYYMMDD DECIMAL(8,0), DEFAULTDATE DATE ) 

    RETURNS DATE
    LANGUAGE SQL
    SPECIFIC DEC8TODATE
    DETERMINISTIC
    CALLED ON NULL INPUT
    SET OPTION DATFMT  = *ISO,
               DATSEP = '-'

BEGIN

   DECLARE CONVERTED DATE;
   
   DECLARE EXIT HANDLER FOR SQLEXCEPTION RETURN DEFAULTDATE;
   
   SET CONVERTED = 
     DATE( INSERT( INSERT( DIGITS( YYYYMMDD ),5,0,'-') ,8,0,'-') );
   
   RETURN COALESCE( CONVERTED, DEFAULTDATE );
   
END;