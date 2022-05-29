--  Generate SQL 
--  Version:                   	V7R3M0 160422 
--  Generated on:              	04/21/20 06:15:58 
--  Relational Database:       	DVLA 
--  Standards Option:          	Db2 for i 
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLADMITSOURCES ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALADSR 
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
	SELECT OID , CODE , DESCRIPTION	FROM	
	( SELECT 0 AS OID ,		TRIM ( QTKEY ) AS CODE ,		
	TRIM ( QTPSD ) AS DESCRIPTION		
	FROM HPADQTPS	
	WHERE QTHSP# = P_FACILITYID	
	UNION ALL	
	SELECT 0 AS OID ,	'' AS CODE ,	
	'' AS DESCRIPTION FROM SYSIBM . SYSDUMMY1 )		AS V 
	( OID , CODE , DESCRIPTION )	ORDER BY CODE ; 
	OPEN CURSOR1 ; END P1  ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE SLALADSR 
TO RMTGRPPRF WITH GRANT OPTION ; 
  
