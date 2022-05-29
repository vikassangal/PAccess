-- *****************************************************************************/
-- * Copyright (C) 2005, Perot Systems Corporation. All right reserved.        */
-- * Developer: Deepthi G (Deepthi.g@blr.pstsi.com)						       */
-- * Started:  04/3/2005                                                       */
-- *****************************************************************************/

-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTTRANSACTIONHEADER');

-- ******************************************************************************************/
-- * SP Definition - PACCESS.SELECTTRANSACTIONHEADER                                        */
-- *    Params     - P_INSURANCETXN	- the number of insurance transactions                  */
-- *    Params     - P_NONINSURANCETXN	- the number of other non-insurances transactions   */
-- ******************************************************************************************/ 

CREATE PROCEDURE PACCESS.SELECTTRANSACTIONHEADERINFORMATION (
	IN P_INSURANCETXN INTEGER, 
	IN P_NONINSURANCETXN INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTTRANSACTIONHEADERINFORMATION 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN ATOMIC				 
		DECLARE CURSOR1 CURSOR FOR 
		SELECT APNXA1 as PATIENTRELATIVERECORDNUMBER, 
		APNXA2 as INSURANCERELATIVERECORDNUMBER, 
		APINLH as INPUTLOGNUMBER
		FROM HPDATA2.HPADAPHD FOR UPDATE;	
		OPEN CURSOR1 ; 

		UPDATE HPDATA2.HPADAPHD SET APNXA1 = APNXA1 + P_NONINSURANCETXN,
		APINLH = APINLH + 1, 
		APNXA2 = APNXA2 + P_INSURANCETXN;					 

	END P1  ;