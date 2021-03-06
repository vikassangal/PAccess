--  Generate SQL 
--  Version:                   	V6R1M0 080215 
--  Generated on:              	01/21/13 01:33:32 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
SET PATH *LIBL ;
  
CREATE PROCEDURE UPDATELASTLOGINDATEFORUSER ( 
	IN P_PBARID VARCHAR(8) , 
	IN P_LOGINDATE VARCHAR(8) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC UPDLASTLOGDATE 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	CLOSQLCSR = *ENDMOD , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	P1 : BEGIN 
  
	UPDATE XADATA.EI0008P 
		SET EIUSDT = P_LOGINDATE WHERE EIEMP# = P_PBARID ; 
  
  
  
END P1  ;
