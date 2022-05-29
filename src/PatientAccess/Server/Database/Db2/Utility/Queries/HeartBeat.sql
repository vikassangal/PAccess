-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  06/21/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTSTATUS');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @NHHSPC - an HSP Code                                      */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTSTATUS 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTSTATUS 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
				DECLARE CURSOR1 CURSOR FOR 
				SELECT IDCOL
				FROM PACCESS . SYSSTAT; 
				OPEN CURSOR1 ; 
				END P1  ;
--go  