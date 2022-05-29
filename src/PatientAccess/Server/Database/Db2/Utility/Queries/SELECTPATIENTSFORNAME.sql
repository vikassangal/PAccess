-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  06/21/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTPATIENTSFORNAME');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @LPLNAME	- Part of a persons last name                  */
-- *    Params     - @LPHSP	- an HSP Code                                      */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTPATIENTSFORNAME ( 
	IN @LPLNAME VARCHAR(32) , 
	IN @LPHSP INTEGER ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTPATIENTSFORNAME 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
				DECLARE CURSOR1 CURSOR FOR 
				SELECT LPPLNM , LPPFNM , LPMRC# AS MRC 
				FROM HPDATA1 . HPADLPU 
				WHERE LPPLNM LIKE CONCAT ( @LPLNAME , '%' ) 
				AND LPHSP# = @LPHSP ; 
				DECLARE CURSOR2 CURSOR FOR 
				SELECT LPADT1 , LPADT2 , LPADT3 , LPLAT 
				FROM HPDATA1 . HPADLPU 
				WHERE LPPLNM LIKE CONCAT ( @LPLNAME , '%' ) 
				AND LPHSP# = @LPHSP ; 
				OPEN CURSOR1 ; 
				OPEN CURSOR2 ; 
				END P1  
--go