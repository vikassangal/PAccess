-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/07/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','DELETEEMPLOYER');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.INSERTNEWEMPLOYER                                 */
-- *    Params     - P_EMCODE	- Employer Code                                */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.DELETEEMPLOYER ( 
	IN P_EMCODE INTEGER) 
	LANGUAGE SQL 
	SPECIFIC PACCESS.DELETEEMPLOYER 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		
		DELETE FROM PADATA.NCEM10P 
		WHERE   EMCODE = P_EMCODE;
		END P1  ;   