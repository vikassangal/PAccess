--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	01/11/08 15:05:28 
--  Relational Database:       	ESTA 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE MPIACCOUNTSFOR ( 
	IN P_MRN INTEGER , 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC MPIACCFOR 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER   
	P1 : BEGIN 
			 
	  
	DECLARE CUR_STR VARCHAR ( 5000 ) ; 
	DECLARE CUR_WHERE VARCHAR ( 2500 ) ; 
	DECLARE CUR_GENDER VARCHAR ( 2500 ) ; 
	DECLARE INSQL VARCHAR ( 10000 ) ; 
	DECLARE CURSOR1 CURSOR FOR 

	SELECT	DISTINCT RWP . RWCNFG AS CONFIDENTIALFLAG , 
	RWP . RWACCT AS ACCOUNTNUMBER , 
	PMP . PMPT#9 AS PURGEDACCOUNTNUMBER , 
	RWP . RWPTYP AS PATIENTTYPE , 
	RWP . RWZB10 AS HOSPITALSERVICECODE , 
	RWP . RWLAD AS ADMITDATE , 
	RWP . RWLAT AS ADMITTIME , 
	RWP . RWLDD AS DISCHARGEDATE , 
	RWP . RWLDT AS DISCHARGETIME , 
	RWP . RWDCOD AS DISCHARGECODE , 
	RWP . RWCL25 AS SITECODE , 
	QCP . QCSST5 AS MULTISITEFLG , 
	RVP . RVMRC# AS ORIGINALMRN1 , 
	NDO . OMMRC# AS ORIGINALMRN2 , 
	LPP . LPACCT AS LPPACCOUNTNUMBER 

	FROM HXMRRWP RWP 
	
	LEFT OUTER JOIN ( SELECT RVMRC# , RVHSP# , RVACCT 
	FROM HXMRRVP WHERE RVHSP# = P_FACILITYID ) RVP 
	ON ( RVP . RVHSP# = RWP . RWHSP# AND RWP . RWACCT = RVP . RVACCT ) 

	LEFT OUTER JOIN ( SELECT OMMRC# , OMHSP# , OMACCT 
	FROM NDOMR#P WHERE OMHSP# = P_FACILITYID ) NDO 
	ON ( NDO . OMHSP# = RWP . RWHSP# AND RWP . RWACCT = NDO . OMACCT ) 

	LEFT OUTER JOIN HPADQCP1 QCP ON ( QCP . QCHSP# = RWP . RWHSP# ) 

	LEFT OUTER JOIN ( SELECT PMPT#9 
	FROM PMP002P 
	WHERE PMHSPC = ( SELECT FFHSPC 
	FROM FF0015P WHERE FFHSPN = P_FACILITYID ) ) PMP 
	ON ( RWP . RWACCT = PMP . PMPT#9 ) 

	LEFT OUTER JOIN HPADLPP LPP 
	ON ( LPP . LPHSP# = RWP . RWHSP# 
	AND LPP . LPACCT = RWP . RWACCT 
	AND LPP . LPMRC# = RWMRC# ) 

	WHERE RWP . RWHSP# = P_FACILITYID 
	AND RWP . RWMRC# = P_MRN ; 
	
   
    --CALL QSYS / QCMDEXC ( 'CHGQRYA QRYOPTLIB(PACCESS)' , 0000000026.00000 ) ; 
                         
                                           
    OPEN CURSOR1 ;                                                  
    END P1  ;                                                       
