-- *****************************************************************************/
-- * Copyright (C) 2005, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kiran Prasanna Kumar  (kirankumar.p@blr.pstsi.com)                           */
-- * Started:  04/1/2005                                                       */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTACCOUNTNUMBERSPAN');

-- *****************************************************************************/
-- * SP Definition - PACCESS.SELECTACCOUNTNUMBERSPAN                           */
-- *    Params     - P_HSP	- an HSP Code                                      */
-- *    Params     - P_SPANSIZE	- the size of the span                         */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTACCOUNTNUMBERSPAN ( 
	IN P_HSP INTEGER,
	IN P_SPANSIZE INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTACCOUNTNUMBERSPAN 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN atomic
		DECLARE CURSOR1 CURSOR FOR
		SELECT QCNAC# AS NEXTACCOUNTNUMBER FROM HPDATA1 . HPADQCP2
		WHERE QCHSP# = P_HSP ;

		OPEN CURSOR1 ;

		UPDATE HPDATA1 . HPADQCP2
		SET QCNAC# = QCNAC# + P_SPANSIZE
		WHERE QCHSP# = P_HSP ;
		
	END P1  ;   