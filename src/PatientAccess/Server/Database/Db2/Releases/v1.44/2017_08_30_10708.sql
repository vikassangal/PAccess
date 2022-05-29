
/*************I********************I***********************************/        
/*  Date      I  Programmer        I  Modification Description        */        
/*************I********************I***********************************/        
/* 09/14/2017  I  Smitha Krishnamurthy    I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                                          
CREATE PROCEDURE SELECTALLSUFFIXCODES( 
	IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELALLSUF                                                       
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                
        DECLARE CURSOR1 CURSOR FOR          
                                            
        SELECT SUFFIXID as OID, 
        SUFFIXCODE as CODE, 
        SUFFIXDESCRIPTION as DESCRIPTION
        FROM            
       (
           SELECT 	DISTINCT
0 AS SUFFIXID,
TRIM(QTKEY) AS SUFFIXCODE,
TRIM(QTGNCDC) AS SUFFIXDESCRIPTION
FROM	HPADQTGN
WHERE	QTHSP# = P_FACILITYID
AND		QTKEY IS NOT NULL
AND		QTKEY <> ''
UNION ALL
SELECT	0 AS SUFFIXID,
'' AS SUFFIXCODE,
'' AS SUFFIXDESCRIPTION
FROM	SYSIBM.SYSDUMMY1
) AS V(SUFFIXID, SUFFIXCODE, SUFFIXDESCRIPTION)
ORDER BY CASE SUFFIXCODE
WHEN  '' 	THEN	1
WHEN 'JR'	THEN	2
WHEN 'SR'	THEN	3
WHEN 'II'	THEN	4
WHEN 'III'	THEN	5
WHEN 'IV'	THEN	6
WHEN 'V'	THEN	7
WHEN 'VI'	THEN	8
WHEN 'VII'	THEN	9
WHEN 'VIII'	THEN	10
WHEN 'IX'	THEN	11
ELSE 99
END;
OPEN CURSOR1 ;
END P1  ;
