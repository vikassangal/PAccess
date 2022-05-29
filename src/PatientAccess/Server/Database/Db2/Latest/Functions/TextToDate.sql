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

SET PATH *LIBL ;

CREATE FUNCTION TEXTTODATE( MMDDYYYY VARCHAR(8) ) 

    RETURNS DATE
    LANGUAGE SQL
    SPECIFIC TEXTTODATE
    DETERMINISTIC
    CALLED ON NULL INPUT

BEGIN

   DECLARE CONVERTED DATE;
   
   DECLARE EXIT HANDLER FOR SQLEXCEPTION RETURN DATE('01/01/0001');
   
   SET CONVERTED = 
     DATE( SUBSTR(MMDDYYYY,1,2) || 
     '/' || 
     SUBSTR(MMDDYYYY,3,2) || 
     '/' || 
     SUBSTR(MMDDYYYY,5,4) );
   
   RETURN COALESCE( CONVERTED, DATE( '01/01/0001' ) );
   
END