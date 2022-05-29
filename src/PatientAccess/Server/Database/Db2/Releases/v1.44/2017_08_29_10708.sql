--  Generate SQL 
--  Version:                   	V7R2M0 140418 
--  Generated on:              	09/14/17 02:12:19 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
SET PATH *LIBL ; 
  
CREATE PROCEDURE FINDPATIENTSFOR ( 
	IN @P_FACILITYID INTEGER , 
	IN @P_MRN INTEGER , 
	IN @P_ACCOUNTNUMBER INTEGER , 
	IN @P_SSN CHAR(9) , 
	IN @P_FNAME VARCHAR(25) , 
	IN @P_LNAME VARCHAR(25) , 
	IN @P_GENDER VARCHAR(1) , 
	IN @P_MONTH INTEGER , 
	IN @P_YEAR INTEGER , 
	IN @P_PHONENUMBER VARCHAR(10) ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC FINDPTSFOR 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DLYPRP = *NO , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	RDBCNNMTH = *RUW , 
	SRTSEQ = *HEX   
	BEGIN DECLARE @V_MEDICALRECORDNUMBER INTEGER ; 
	DECLARE @V_QUERYTEMPLATE VARCHAR ( 8192 ) ; 
	DECLARE @V_DEMOFILTER VARCHAR ( 2048 ) DEFAULT ' ' ; 
	DECLARE @V_ALIASFILTER VARCHAR ( 2048 ) DEFAULT ' ' ; 
	DECLARE SEARCHCURSOR CURSOR WITH RETURN FOR SEARCHSTATEMENT ; 
	SET @V_QUERYTEMPLATE = 'SELECT DISTINCT  
	DEMOGRAPHICS.MDSSN9                AS SSN,                                      
	DEMOGRAPHICS.MDSEX                 AS SEXCODE,                                  
	DEMOGRAPHICS.MDMRC#                AS MRN,                                      
	DEMOGRAPHICS.MDDOB8                AS DOB,                                     
	1                                  AS TYPEOFNAMEID,                             
	TRIM ( DEMOGRAPHICS.MDPFNM )       AS DISPLAYFIRSTNAME,                         
	TRIM ( DEMOGRAPHICS.MDPLNM )       AS DISPLAYLASTNAME,                          
	TRIM ( DEMOGRAPHICS.MDPMI )        AS DISPLAYMIDDLEINITIAL,                     
	DEMOGRAPHICS.MDPGNRN      AS TITLE,                                                
	TRIM ( ALIASES.AKPFNM )            AS FIRSTNAME,                                
	TRIM ( ALIASES.AKPLNM )            AS LASTNAME,                                 
	''''                               AS MIDDLEINITIAL,                            
	DEMOGRAPHICS.MDMADR                AS PHYSICALSTREET,                           
	DEMOGRAPHICS.MDMCIT                AS PHYSICALCITY,                             
	DEMOGRAPHICS.MDMSTE                AS PHYSICALSTATE,                            
	RTRIM ( DEMOGRAPHICS.MDMZPA ) ||                                                
	RTRIM ( DEMOGRAPHICS.MDMZ4A )      AS PHYSICALZIP,                              
	DEMOGRAPHICS.MDPADR                AS MAILINGSTREET,                            
	DEMOGRAPHICS.MDPCIT                AS MAILINGCITY,                              
	DEMOGRAPHICS.MDPSTE                AS MAILINGSTATE,                             
	RTRIM( DEMOGRAPHICS.MDPZPA ) ||                                                 
	RTRIM( DEMOGRAPHICS.MDPZ4A )       AS MAILINGZIP                              
	FROM                                                                               
	HPADMDP DEMOGRAPHICS                                                         
	LEFT OUTER JOIN                                                                   
	NMNHAKAP ALIASES                                                              
	ON                                                                                
	DEMOGRAPHICS.MDHSP# = ALIASES.AKHSP#                                                
	AND                                                                         
	DEMOGRAPHICS.MDMRC# = ALIASES.AKMRC#                                          
	WHERE                                                                             
	{1}                                                                                                                                                          
	UNION                                                                                                                                                         
	SELECT DISTINCT                                            
	DEMOGRAPHICS.MDSSN9                AS SSN,                                      
	DEMOGRAPHICS.MDSEX                 AS SEXCODE,                                  
	DEMOGRAPHICS.MDMRC#                AS MRN,                                      
	DEMOGRAPHICS.MDDOB8                AS DOB,                                      
	2                                  AS TYPEOFNAMEID,                             
	TRIM ( ALIASES.AKPFNM )            AS DISPLAYFIRSTNAME,                         
	TRIM ( ALIASES.AKPLNM )            AS DISPLAYLASTNAME,                          
	''''                               AS DISPLAYMIDDLEINITIAL,                     
	DEMOGRAPHICS.MDPGNRN      AS TITLE,                                                
	TRIM ( DEMOGRAPHICS.MDPFNM )       AS FIRSTNAME,                                
	TRIM ( DEMOGRAPHICS.MDPLNM )       AS LASTNAME,                                 
	TRIM ( DEMOGRAPHICS.MDPMI )        AS MIDDLEINITIAL,                            
	DEMOGRAPHICS.MDMADR                AS PHYSICALSTREET,                           
	DEMOGRAPHICS.MDMCIT                AS PHYSICALCITY,                             
	DEMOGRAPHICS.MDMSTE                AS PHYSICALSTATE,                            
	RTRIM ( DEMOGRAPHICS.MDMZPA ) ||                                                
	RTRIM ( DEMOGRAPHICS.MDMZ4A )      AS PHYSICALZIP,                              
	DEMOGRAPHICS.MDPADR                AS MAILINGSTREET,                            
	DEMOGRAPHICS.MDPCIT                AS MAILINGCITY,                              
	DEMOGRAPHICS.MDPSTE                AS MAILINGSTATE,                             
	RTRIM ( DEMOGRAPHICS.MDPZPA ) ||                                                
	RTRIM ( DEMOGRAPHICS.MDPZ4A )      AS MAILINGZIP                              
	FROM                                                                              
	HPADMDP DEMOGRAPHICS                                                          
	LEFT OUTER JOIN                                                                    
	NMNHAKAP ALIASES                                                             
	ON                                                                                
	DEMOGRAPHICS.MDHSP# = ALIASES.AKHSP#                                                
	AND                                                                         
	DEMOGRAPHICS.MDMRC# = ALIASES.AKMRC#                                          
	WHERE                                                                             
	{2}                                                                           
	ORDER BY                                                                          
	DISPLAYLASTNAME,                                                                
	DISPLAYFIRSTNAME,                                                               
	DISPLAYMIDDLEINITIAL,                                                           
	TYPEOFNAMEID' ; 
	IF @P_MRN IS NOT NULL THEN 
	SET @V_MEDICALRECORDNUMBER = @P_MRN ; 
	ELSEIF @P_ACCOUNTNUMBER IS NOT NULL 
	THEN SET @V_MEDICALRECORDNUMBER = 
	( SELECT DISTINCT LPMRC# FROM 
	 HPADLPP WHERE 
	LPACCT = @P_ACCOUNTNUMBER AND 
	LPHSP# = @P_FACILITYID ) ; 
	IF @V_MEDICALRECORDNUMBER IS NULL THEN 
	SET @V_MEDICALRECORDNUMBER = 
	( SELECT MDMRC# FROM 
	 HPADMDP WHERE 
	MDACCT = @P_ACCOUNTNUMBER AND 
	MDHSP# = @P_FACILITYID 
	ORDER BY MDLAD DESC FETCH FIRST ROW ONLY ) ; 
	END IF ; 
	IF @V_MEDICALRECORDNUMBER IS NULL THEN 
	RETURN 0 ; END IF ; END IF ; 
	IF @V_MEDICALRECORDNUMBER IS NULL AND @P_SSN IS NOT NULL 
	THEN SET @V_DEMOFILTER = 
	' ( DEMOGRAPHICS.MDHSP# = ' || @P_FACILITYID || 
	' AND DEMOGRAPHICS.MDMRC# IN ' || 
	'( SELECT MDMRC# FROM HPADMDP 
	WHERE MDSSN9 = ''' || @P_SSN || ''' 
	AND MDHSP# = ' || @P_FACILITYID || ' ) ) ' ; 
	SET @V_ALIASFILTER = 
	' ( ALIASES.AKHSP# = ' || @P_FACILITYID || 
	' AND ALIASES.AKMRC# IN ' || 
	'( SELECT MDMRC# FROM 
	HPADMDP WHERE 
	MDSSN9 = ''' || @P_SSN || ''' AND 
	MDHSP# = ' || @P_FACILITYID || ' ) ) ' ; 
	ELSEIF @V_MEDICALRECORDNUMBER IS NULL 
	AND @P_SSN IS NULL AND 
	@P_PHONENUMBER IS NOT NULL 
	THEN SET @V_DEMOFILTER = 
	' ( DEMOGRAPHICS.MDHSP# = ' || 
	@P_FACILITYID || ' AND DEMOGRAPHICS.MDMRC# IN 
	' || '( SELECT MDMRC# FROM HPADMDP WHERE 
	(  DEMOGRAPHICS.MDPPH#   = ''' || 
	TRIM ( SUBSTRING ( @P_PHONENUMBER , 4 , 7 ) ) || 
	''' AND  DEMOGRAPHICS.MDPACD  =''' || 
	TRIM ( SUBSTRING ( @P_PHONENUMBER , 1 , 3 ) ) || 
	''' AND MDHSP# = ' || @P_FACILITYID || ' ) OR                                                                            
	(                                                                              
	 DEMOGRAPHICS.MDCLPH  =''' || 
	 @P_PHONENUMBER || ''' 
	 AND MDHSP# = ' || @P_FACILITYID || ' ) )) ' ; 
	 SET @V_ALIASFILTER = ' ( ALIASES.AKHSP# = ' || 
	 @P_FACILITYID || ' AND ALIASES.AKMRC# IN ' || 
	 '( SELECT MDMRC# FROM HPADMDP 
	 WHERE 
	 (  DEMOGRAPHICS.MDPPH#   = ''' || 
	 TRIM ( SUBSTRING ( @P_PHONENUMBER , 4 , 7 ) ) || ''' 
	 AND  DEMOGRAPHICS.MDPACD  =''' || 
	 TRIM ( SUBSTRING ( @P_PHONENUMBER , 1 , 3 ) ) || 
	 ''' AND MDHSP# = ' || @P_FACILITYID || ' ) OR                                            
	 (                                                                               
	 DEMOGRAPHICS.MDCLPH  =''' || @P_PHONENUMBER || ''' 
	 AND MDHSP# = ' || @P_FACILITYID || ' ) )) ' ; 
	 ELSEIF @V_MEDICALRECORDNUMBER IS NOT NULL 
	 THEN SET @V_DEMOFILTER =
	 ' ( DEMOGRAPHICS.MDHSP# = ' || @P_FACILITYID || ' 
	 AND DEMOGRAPHICS.MDMRC# = ' || @V_MEDICALRECORDNUMBER || ' ) ' ; 
	 SET @V_ALIASFILTER = ' ( DEMOGRAPHICS.MDHSP# = ' || @P_FACILITYID || ' 
	 AND DEMOGRAPHICS.MDMRC# = ' || @V_MEDICALRECORDNUMBER || ' ) 
	 AND ( ALIASES.AKHSP# = ' || @P_FACILITYID || ' 
	 AND ALIASES.AKMRC# = ' || @V_MEDICALRECORDNUMBER || ' ) ' ; 
	 ELSE SET @V_DEMOFILTER = ' 
	 ( DEMOGRAPHICS.MDHSP# = ' || @P_FACILITYID || ' ) ' ; 
	 SET @V_ALIASFILTER = ' 
	 ( DEMOGRAPHICS.MDHSP# = ' || @P_FACILITYID || ' ) ' ; 
	 END IF ; IF @P_LNAME IS NOT NULL THEN 
	 SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( DEMOGRAPHICS.MDPLSN LIKE 
	 (''' || UPPER ( @P_LNAME ) || '%'') ) ' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( ALIASES.AKPLSN LIKE (''' || 
	 UPPER ( @P_LNAME ) || '%'') ) ' ; 
	 END IF ; IF @P_FNAME IS NOT NULL 
	 THEN SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( DEMOGRAPHICS.MDPFSN LIKE (''' || 
	 UPPER ( @P_FNAME ) || '%'') ) ' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( ALIASES.AKPFSN LIKE (''' || 
	 UPPER ( @P_FNAME ) || '%'') ) ' ; 
	 END IF ; IF @P_GENDER IS NOT NULL 
	 THEN SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( DEMOGRAPHICS.MDSEX = ''' || 
	 UPPER ( @P_GENDER ) || ''') ' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( DEMOGRAPHICS.MDSEX = ''' || 
	 UPPER ( @P_GENDER ) || ''') ' ; 
	 END IF ; IF @P_MONTH IS NOT NULL 
	 AND @P_YEAR IS NOT NULL 
	 THEN SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) 
	 BETWEEN (DATE(''' || @P_MONTH || '/01/' 
	 || @P_YEAR || ''') - 2 YEARS) AND 
	 LAST_DAY((DATE(''' || 
	 @P_MONTH || '/01/' || 
	 @P_YEAR || ''') + 2 YEARS)) ) 
	 AND ' || '( 
	 MONTH( 
	 TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' 
	 || @P_MONTH || ')' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) 
	 BETWEEN (DATE(''' || @P_MONTH || '/01/' || @P_YEAR || ''') - 2 YEARS) 
	 AND 
	 LAST_DAY((DATE(''' || @P_MONTH || '/01/' || @P_YEAR || ''') + 2 YEARS)) )
	  AND 
	 ' || '( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = ' || @P_MONTH || ')' ; 
	 ELSEIF @P_YEAR IS NOT NULL THEN SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) 
	 BETWEEN (DATE(''01/01/' || @P_YEAR || ''') - 2 YEARS) 
	 AND (DATE(''12/31/' || @P_YEAR || ''') + 2 YEARS ) ) ' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) 
	 BETWEEN (DATE(''01/01/' || @P_YEAR || ''') - 2 YEARS) 
	 AND (DATE(''12/31/' || @P_YEAR || ''') + 2 YEARS ) ) ' ; 
	 ELSEIF @P_MONTH IS NOT NULL 
	 THEN SET @V_DEMOFILTER = @V_DEMOFILTER || ' 
	 AND ( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = 
	 ' || @P_MONTH || ') ' ; 
	 SET @V_ALIASFILTER = @V_ALIASFILTER || ' 
	 AND ( MONTH( TEXTTODATE( DEMOGRAPHICS.MDDOB8 ) ) = 
	 ' || @P_MONTH || ') ' ; 
	 END IF ; 
	 SET @V_QUERYTEMPLATE = 
	 REPLACE ( @V_QUERYTEMPLATE , '{1}' , 
	 @V_DEMOFILTER ) ; 
	 SET @V_QUERYTEMPLATE = 
	 REPLACE ( @V_QUERYTEMPLATE , '{2}' , 
	 @V_ALIASFILTER ) ; 
	 PREPARE SEARCHSTATEMENT 
	 FROM @V_QUERYTEMPLATE ; 
	 OPEN SEARCHCURSOR ; END  ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE FINDPTSFOR 
TO RMTGRPPRF ; 
  
GRANT ALTER , EXECUTE   
ON SPECIFIC PROCEDURE FINDPTSFOR 
TO XA2PGMR ;
