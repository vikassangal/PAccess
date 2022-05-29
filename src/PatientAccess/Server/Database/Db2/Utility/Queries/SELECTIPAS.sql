-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  06/21/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTALLIPAS');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @NHHSPC - an HSP Code                                      */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTALLIPAS ( 
	IN @NHHSPC VARCHAR(3) ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTALLIPAS 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
				DECLARE CURSOR1 CURSOR FOR 
				SELECT NHIPA, NHIPAC, NHIPAN, NHIPCN 
				FROM NMDATA1 . NH0301P 
				WHERE  NHHSPC = @NHHSPC ; 
				OPEN CURSOR1 ; 
				END P1  ;
--go 
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTIPAS');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @IPANAME	- Part of a persons last name                  */
-- *    Params     - @NHHSPC	- an HSP Code                                  */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTIPAS ( 
	IN @NHHSPC VARCHAR(3),
	IN @IPANAME VARCHAR(32) ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTIPAS 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
				DECLARE CURSOR1 CURSOR FOR 
				SELECT NHIPA, NHIPAC, NHIPAN, NHIPCN 
				FROM NMDATA1 . NH0301P 
				WHERE NHIPAN LIKE CONCAT ( @IPANAME , '%' ) 
				AND NHHSPC = @NHHSPC ; 
				OPEN CURSOR1 ; 
				END P1  ;
--go 

CALL DROP_PROC('PACCESS','SELECTSPECIFICIPACLINIC');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @IPANAME	- Part of a persons last name                  */
-- *    Params     - @NHHSPC	- an HSP Code                                  */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTSPECIFICIPACLINIC ( 
	IN @NHHSPC VARCHAR(3),
	IN @IPACODE VARCHAR(5),
	IN @CLINICCODE VARCHAR(2) ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTSPECIFICIPACLINIC 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
				DECLARE CURSOR1 CURSOR FOR 
				SELECT NHIPA, NHIPAC, NHIPAN, NHIPCN 
				FROM NMDATA1 . NH0301P 
				WHERE NHIPA = @IPACODE
				AND NHIPAC = @CLINICCODE
				AND NHHSPC = @NHHSPC ; 
				OPEN CURSOR1 ; 
				END P1  ;
--go 