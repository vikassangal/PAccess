-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  09/29/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the func if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','CREATEPATIENTTYPE');
--go

-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - @LPLNAME	- Part of a persons last name                  */
-- *    Params     - @LPHSP	- an HSP Code                                      */
-- *****************************************************************************/

CREATE FUNCTION PACCESS.CREATEPATIENTTYPE (
	LPPSTO varchar(1),
	LPPSTI varchar(1),
	LPFBIL varchar(1),
	LPVIS# DECIMAL,
	LPPOLN varchar(1),
	LPUBAL DECIMAL,
	LPMSV varchar(3)
	) 
	RETURNS CHAR(8) 
	LANGUAGE SQL 
	CONTAINS SQL
	SPECIFIC PACCESS.CREATEPATIENTTYPE 
	DETERMINISTIC 
	CALLED ON NULL INPUT 
	NO EXTERNAL ACTION 
	BEGIN ATOMIC

	DECLARE INTERNALSTATUSCODE CHAR ( 1 ) ; 
	DECLARE RETURNVAL CHAR ( 8 ) DEFAULT ''; 

	if ( LPPSTI = 'A' AND LPPOLN = 'P' ) then
		set INTERNALSTATUSCODE = 'M';
	elseif ( LPPSTI = 'A' AND LPPOLN <> 'P' AND LPUBAL = 0) then
		set INTERNALSTATUSCODE = 'A';
	elseif ( LPPSTI = 'A' AND LPPOLN <> 'P' AND LPUBAL <> 0) then
		set INTERNALSTATUSCODE = 'I';
	elseif ( LPPSTI = 'B' ) then
		set INTERNALSTATUSCODE = 'B';
	elseif ( LPPSTI = 'C' OR LPPSTI = 'J' ) then
		set INTERNALSTATUSCODE = 'C';
	elseif ( LPPSTI = 'B' AND LPPSTO = ' ' ) then
		set INTERNALSTATUSCODE = 'G';
	elseif ( LPPSTI = 'H' ) then
		set INTERNALSTATUSCODE = 'H';
	elseif ( LPPSTO = 'K' ) then
		set INTERNALSTATUSCODE = 'K';
	elseif ( LPPSTO = 'E' AND LPFBIL = 'Y' ) then
		set INTERNALSTATUSCODE = 'L';
	elseif ( LPPSTO = 'E' AND LPFBIL <> 'Y' ) then
		set INTERNALSTATUSCODE = 'E';
	elseif ( LPPSTO = 'F' AND LPFBIL = 'Y' ) then
		set INTERNALSTATUSCODE = 'L';
	elseif ( LPPSTO = 'F' AND LPFBIL <> 'Y' ) then
		set INTERNALSTATUSCODE = 'D';
	elseif ( LPPSTO = 'D' AND LPVIS# = 0 ) then
		set INTERNALSTATUSCODE = 'D';
	elseif ( LPPSTO = 'D' AND LPVIS# <> 0 ) then
		set INTERNALSTATUSCODE = 'E';
	end if;

	if ( INTERNALSTATUSCODE = 'M' ) then
		set returnVal = 'PRE-PUR' ;
	elseif ( INTERNALSTATUSCODE = 'A' ) then
		set returnVal = 'PRE-N/C' ;
	elseif ( INTERNALSTATUSCODE = 'I' ) then
		set returnVal = 'PRE-CHG' ;
	elseif ( INTERNALSTATUSCODE = 'B' ) then
		set returnVal = 'INP-INH' ;
	elseif ( INTERNALSTATUSCODE = 'C' ) then
		set returnVal = 'INP-DIS' ;
	elseif ( INTERNALSTATUSCODE = 'E' ) then
		set returnVal = 'OUT-REC' ;
	elseif ( INTERNALSTATUSCODE = 'G' ) then
		set returnVal = 'PRE-CAN' ;
	elseif ( INTERNALSTATUSCODE = 'H' ) then
		set returnVal = 'INP-CAN' ;
	elseif ( INTERNALSTATUSCODE = 'K' ) then
		set returnVal = 'OUT-PRE' ;
	elseif ( INTERNALSTATUSCODE = 'L' ) then
		set returnVal = 'OUT-FIN' ;
	elseif ( INTERNALSTATUSCODE = 'D' ) then
		set returnVal = 'O HSV' || LPMSV ;
	end if;





	  

	RETURN returnVal ; 

	  

	END;
  

COMMENT ON SPECIFIC FUNCTION PACCESS.CREATEPATIENTTYPE 

	IS 'Create a string that represnets the paitent type' ;
