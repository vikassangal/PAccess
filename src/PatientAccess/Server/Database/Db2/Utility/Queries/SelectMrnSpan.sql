-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  02/3/2005                                                       */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTMRNSPAN');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.SELECTMRNSPAN                                     */
-- *    Params     - P_HSP	- an HSP Code                                      */
-- *    Params     - P_SPANSIZE	- the size of the span                         */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTMRNSPAN ( 
	IN P_HSP INTEGER,
	IN P_SPANSIZE INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTMRNSPAN 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN atomic
		DECLARE cursor1 CURSOR FOR 
		SELECT QCNMR# as NEXTMRN FROM HPDATA1.HPADQCP2 
		where QCHSP# = P_HSP ;
		OPEN CURSOR1 ; 

		Update HPDATA1.HPADQCP2
		SET QCNMR# = QCNMR# + P_SPANSIZE
		WHERE QCHSP# = P_HSP;
	END P1  ;  