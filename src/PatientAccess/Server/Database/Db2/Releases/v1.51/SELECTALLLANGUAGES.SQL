﻿--  Generate SQL 
--  Version:                   	V7R3M0 160422 
--  Generated on:              	01/23/20 12:14:54 
--  Relational Database:       	DVLA 
--  Standards Option:          	Db2 for i 
SET PATH *LIBL ; 

CREATE PROCEDURE SELECTALLLANGUAGES ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALALNGS 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	CLOSQLCSR = *ENDACTGRP , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DLYPRP = *NO , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	RDBCNNMTH = *RUW , 
	SRTSEQ = *HEX   
	P1 : BEGIN DECLARE CURSOR1 CURSOR FOR 
		( SELECT 0 AS OID ,		 
		'' AS CODE ,		 
		'' AS DESCRIPTION		 
		FROM SYSIBM . SYSDUMMY1 ) 
		UNION ALL 
		( SELECT 0 AS OID , 
		TRIM ( NDKEY ) AS CODE , 
		TRIM ( NDLNDS ) AS DESCRIPTION 
		FROM NDLN01P 
		WHERE NDHSP# = P_FACILITYID 
		ORDER BY CASE 
			WHEN NDLNDS = 'ENGLISH' 
			THEN 0 
			WHEN NDLNDS = 'SPANISH' 
			THEN 1 
			ELSE 2 END , DESCRIPTION ASC ) ; 
	OPEN CURSOR1 ; END P1  ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE SLALALNGS 
TO RMTGRPPRF WITH GRANT OPTION ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE SLALALNGS 
TO TURNOVER WITH GRANT OPTION ;
