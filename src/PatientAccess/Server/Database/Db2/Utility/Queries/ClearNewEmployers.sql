-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/07/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','CLEARNEWEMPLOYERS');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.CLEARNEWEMPLOYERS                                 */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.CLEARNEWEMPLOYERS 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.CLEARNEWEMPLOYERS 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		DELETE FROM PACCESS.EMPTXN;
	END P1  ;   
	
	 