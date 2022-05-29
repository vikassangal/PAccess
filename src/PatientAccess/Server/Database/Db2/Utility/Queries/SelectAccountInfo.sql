-- *****************************************************************************/
-- * PatientPackage                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  06/21/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTACCOUNTINFO');
--go



-- *****************************************************************************/
-- * SP Definition - PSCKJS.KJSTEST1                                           */
-- *    Params     - P_HSP	- an HSP Code                                      */
-- *    Params     - P_MRC	- a medical record number                          */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTACCOUNTINFO ( 
	IN P_HSP INTEGER,
	IN P_MRC INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTACCOUNTINFO 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		DECLARE CURSOR1 CURSOR FOR 
		SELECT LPHSP# as HSPNUMBER , 
			LPACCT AS ACCOUNTNUMBER,
			LPPLNM as LASTNAME, 
			LPPFNM as FIRSTNAME, 
			LPADT1 as ADMITMONTH, 
			LPADT2 as ADMITDAY,
			LPADT3 as ADMITYEAR, 
			LPLAT  as TIMEOFADMISSION,
			LPLDD as DISCHARGEDATE,
			LPFC as FINANCIALCLASSCODE, 
			LPMSV as HOSPITALSERVICECODE,
			
            LPVALU as HASVALUABLE,
            LPDCOD as DISCHARGEDISPOSITIONCODE,
            LPABST as ABSTRACTEXISTS,
            LPDSFL as DISCHARGEPENDINGFLAG,
            LPNS   as NURSESTATION,
            LPROOM as ROOM,             
            LPBED  as BED,
			
			s.QTKEY as MEDICALSERVICECODE, 
			s.QTMSVD as MEDICALSERVICEDESC,
			t.QTKEY as PATIENTTYPECODE, 
			t.QTTAD as PATIENTTYPEDESC,
			l.LFRCID as LOCKINDICATOR,
			Date(LPLDDC || substr('0' || trim(char(LPADT3)),length('0' || trim(char(LPADT3))) - 1, 2) || '-' || trim(char(LPADT1)) || '-' || trim(char(LPADT2))) as SortDate,
			createpatienttype(LPPSTO,LPPSTI,LPFBIL,LPVIS#,LPPOLN,LPUBAL,LPMSV) as PATIENTTYPE
		FROM HPDATA1 . HPADLPP a
			left join HPDATA1 . HPADQTMS s on s.QTHSP# = a.LPHSP#
			left join HPDATA1 . HPADQTTA t on t.QTHSP# = a.LPHSP#
			left outer join XADATA . NQHRLFP l on (DECIMAL(LFKEY) = a.LPACCT AND LFHSP# = a.LPHSP#)
		WHERE a.LPHSP# = P_HSP
		AND a.LPMSV = s.QTKEY
		AND t.QTKEY = a.LPPTYP
		AND a.LPMRC# = P_MRC
		ORDER BY SortDate desc, ACCOUNTNUMBER desc; 
		OPEN CURSOR1 ; 
		END P1  ; 