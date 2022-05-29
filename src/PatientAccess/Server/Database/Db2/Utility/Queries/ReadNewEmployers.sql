-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/07/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','READNEWEMPLOYERS');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.READNEWEMPLOYER                                   */
-- *****************************************************************************/

CREATE PROCEDURE PACCESS.READNEWEMPLOYERS
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.READNEWEMPLOYERS
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		DECLARE CURSOR1 CURSOR FOR 
		SELECT  EMFUUN, EMNAME, EMURFG, EMADDT, EMLMDT, EMDLDT,
			EMCODE, EMACNT, EMUSER, EMNEID, TXNDATE, ACTION
 		FROM PACCESS.EMPTXN EMP
		WITH CS KEEP LOCKS;
		OPEN CURSOR1;
	END P1;


	