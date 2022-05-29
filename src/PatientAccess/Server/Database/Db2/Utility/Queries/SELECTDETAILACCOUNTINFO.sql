-- *****************************************************************************/
-- * SELECTDETAILACCOUNTINFO                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  02/07/2005                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTDETAILACCOUNTINFO');
--go



-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - P_HSP	- an HSP Code                                      */
-- *    Params     - P_ACCOUNTNUMBER	- an account number                          */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTDETAILACCOUNTINFO ( 
	IN P_HSP INTEGER,
	IN P_ACCOUNTNUMBER INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTDETAILACCOUNTINFO 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		DECLARE CURSOR1 CURSOR FOR 
		SELECT LPHSP# as HSPNUMBER ,
			LPACCT AS ACCOUNTNUMBER,
			LPALOC as ACCIDENTLOCATION, 
			LPATIM as ACCIDENTTIME,
			LPACTP as ACCIDENTTYPE,
			LPDIAG as DIAGNOSIS,
			LPACST as AUTOACCIDENTSTATE,
			LPACCN as ACCIDENTCOUNTRYCODE,
			LPLAT  as TIMEOFADMISSION,
			LPOC01 AS OCCURANCECODE1,
			LPOC02 AS OCCURANCECODE2,
			LPOC03 AS OCCURANCECODE3,
			LPOC04 AS OCCURANCECODE4,
			LPOC05 AS OCCURANCECODE5,
			LPOC06 AS OCCURANCECODE6,
			LPOC07 AS OCCURANCECODE7,
			LPOC08 AS OCCURANCECODE8,
			LPOA01 AS OCCURANCEDATE1,
			LPOA02 AS OCCURANCEDATE2,
			LPOA03 AS OCCURANCEDATE3,
			LPOA04 AS OCCURANCEDATE4,
			LPOA05 AS OCCURANCEDATE5,
			LPOA06 AS OCCURANCEDATE6,
			LPOA07 AS OCCURANCEDATE7,
			LPOA08 AS OCCURANCEDATE8,
			LPCLVS AS CLERGYVISIT,
			LPLMEN AS LASTMENSTRATION,
			sup.RVPRGI as ISPREGNANT
		FROM HPDATA1 . HPADLPP a
		LEFT OUTER JOIN HPDATA1 . HXMRRV2P sup ON (sup.RVHSP# = a.LPHSP# and sup.RVPT#9 = a.LPACCT)
		WHERE a.LPHSP# = P_HSP
		AND a.LPACCT = P_ACCOUNTNUMBER; 
		
		OPEN CURSOR1 ; 
		
		END P1  ;  