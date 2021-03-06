﻿--  Generate SQL 
--  Version:                   	V7R3M0 160422 
--  Generated on:              	03/04/20 03:01:49 
--  Relational Database:       	DVLA 
--  Standards Option:          	Db2 for i 
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTOCCURANCECODEWITH ( 
IN P_FACILITYID INTEGER , 
IN P_CODE VARCHAR(10) ) 
DYNAMIC RESULT SETS 1 
LANGUAGE SQL 
SPECIFIC SLOCCDWI 
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
P1 : BEGIN 
DECLARE CURSOR1 CURSOR FOR 
SELECT 0 AS OID ,	
TRIM ( QTKEY ) AS CODE ,	
TRIM ( QTOCD ) AS DESCRIPTION	
FROM HPADQTOC	
WHERE QTHSP# = P_FACILITYID	
AND QTKEY = P_CODE	
ORDER BY QTKEY ; 
OPEN CURSOR1 ; END P1  ;
