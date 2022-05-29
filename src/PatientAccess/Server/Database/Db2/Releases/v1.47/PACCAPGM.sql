﻿--  Generate SQL 
--  Version:                   	V7R2M0 140418 
--  Generated on:              	08/09/18 01:01:57 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
CREATE TABLE PACCESS.PACCAPGM ( 
	APIDWS CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APIDID CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APRR# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APSEC2 CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	APHSP# DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	APPREC DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APACCT DECIMAL(9, 0) NOT NULL DEFAULT 0 , 
	APMRC# DECIMAL(9, 0) NOT NULL DEFAULT 0 , 
	APGAR# DECIMAL(9, 0) NOT NULL DEFAULT 0 , 
	APPGAR NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APGLNM CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APGFNM CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	APGAD1 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APGAD1E1 CHAR(50) CCSID 37 NOT NULL DEFAULT '' , 
	APGAD1E2 CHAR(50) CCSID 37 NOT NULL DEFAULT '' , 
	APGAD2 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APGCIT CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	APGZIP DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	APGCNT CHAR(6) CCSID 37 NOT NULL DEFAULT '' , 
	APGACD DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	APGPH# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APENM CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APEADR CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APECIT CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	APESTE CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APEZIP DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	APEZP4 DECIMAL(4, 0) NOT NULL DEFAULT 0 , 
	APEACD DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	APEPH# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APGOCC CHAR(14) CCSID 37 NOT NULL DEFAULT '' , 
	FLR201 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR202 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR203 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APFR01 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APFR02 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APFR03 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APGP01 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APGP02 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APGP03 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO01 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO02 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO03 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APSB01 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APSB02 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APSB03 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APRL01 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APRL02 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APRL03 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APNST CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APNOTE CHAR(20) CCSID 37 NOT NULL DEFAULT '' , 
	APGSTE CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APGZP4 DECIMAL(4, 0) NOT NULL DEFAULT 0 , 
	APGCNY DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	APGSSN CHAR(11) CCSID 37 NOT NULL DEFAULT '' , 
	APGEID CHAR(11) CCSID 37 NOT NULL DEFAULT '' , 
	APGESC CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	FLR204 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR205 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR206 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APFR04 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APFR05 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APFR06 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APGP04 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APGP05 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APGP06 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO04 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO05 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APPO06 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	APSB04 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APSB05 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APSB06 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APRL04 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APRL05 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APRL06 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABGLNM CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABGFNM CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	ABGAD1 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABGAD2 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABGCIT CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	ABGZIP DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	ABGCNT CHAR(6) CCSID 37 NOT NULL DEFAULT '' , 
	ABGACD DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	ABGPH# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	ABENM CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABEADR CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABECIT CHAR(15) CCSID 37 NOT NULL DEFAULT '' , 
	ABESTE CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	ABEZIP DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	ABEZP4 DECIMAL(4, 0) NOT NULL DEFAULT 0 , 
	ABEACD DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	ABEPH# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	ABGOCC CHAR(14) CCSID 37 NOT NULL DEFAULT '' , 
	FLR207 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR208 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR209 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	ABFR01 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABFR02 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABFR03 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABGP01 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABGP02 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABGP03 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO01 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO02 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO03 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB01 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB02 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB03 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL01 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL02 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL03 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABNST CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	ABNOTE CHAR(20) CCSID 37 NOT NULL DEFAULT '' , 
	ABGSTE CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	ABGZP4 DECIMAL(4, 0) NOT NULL DEFAULT 0 , 
	ABGCNY DECIMAL(3, 0) NOT NULL DEFAULT 0 , 
	ABGSSN CHAR(11) CCSID 37 NOT NULL DEFAULT '' , 
	ABGEID CHAR(11) CCSID 37 NOT NULL DEFAULT '' , 
	ABGESC CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	FLR210 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR211 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	FLR212 CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	ABFR04 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABFR05 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABFR06 NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	ABGP04 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABGP05 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABGP06 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO04 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO05 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABPO06 CHAR(12) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB04 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB05 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABSB06 CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL04 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL05 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	ABRL06 CHAR(8) CCSID 37 NOT NULL DEFAULT '' , 
	APLML DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	APLMD DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	APLUL# DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APACFL CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APTTME DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	APINLG DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	APBYPS CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APSWPY NUMERIC(2, 0) NOT NULL DEFAULT 0 , 
	APDRL# CHAR(17) CCSID 37 NOT NULL DEFAULT '' , 
	APGLOE CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	APUN CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	ABDRL# CHAR(17) CCSID 37 NOT NULL DEFAULT '' , 
	ABGLOE CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	ABUN CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	APGPSM CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	ABGPSM CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APGLR CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	ABGLR CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	APGLRO CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	ABGLRO CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APIN01 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APIN02 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APIN03 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APIN04 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APIN05 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APIN06 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN01 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN02 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN03 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN04 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN05 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABIN06 CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APTDAT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	APCLRK CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	ABCLRK CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	APZDTE CHAR(6) CCSID 37 NOT NULL DEFAULT '' , 
	APZTME CHAR(6) CCSID 37 NOT NULL DEFAULT '' , 
	APGZPA CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APGZ4A CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	APGCUN CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	ABGZPA CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABGZ4A CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	ABGCUN CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APEZPA CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	APEZ4A CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	APECUN CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	ABEZPA CHAR(5) CCSID 37 NOT NULL DEFAULT '' , 
	ABEZ4A CHAR(4) CCSID 37 NOT NULL DEFAULT '' , 
	ABECUN CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APLAST CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APOMR# CHAR(11) CCSID 37 NOT NULL DEFAULT '' , 
	APAPP# CHAR(7) CCSID 37 NOT NULL DEFAULT '' , 
	APGEML CHAR(64) CCSID 37 NOT NULL DEFAULT '' , 
	ABGEML CHAR(64) CCSID 37 NOT NULL DEFAULT '' , 
	ABGMI CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APGMI CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	ABGSEX CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APGSEX CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	APGCPH CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	ABGCPH CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APWSIR CHAR(20) CCSID 37 NOT NULL DEFAULT '' , 
	APSECR CHAR(20) CCSID 37 NOT NULL DEFAULT '' , 
	APORR1 CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APORR2 CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	APORR3 CHAR(32) CCSID 37 NOT NULL DEFAULT '' , 
	APGGNRN CHAR(6) CCSID 37 NOT NULL DEFAULT '' , 
	ABPGNRN CHAR(6) CCSID 37 NOT NULL DEFAULT '' )   
	; 
  
LABEL ON TABLE PACCESS.PACCAPGM 
	IS 'TEST GUARANTOR MAINTENANCE TRANSACTION' ; 
  
LABEL ON COLUMN PACCESS.PACCAPGM 
( APIDWS IS 'WS-ID' , 
	APIDID IS 'TRANS               ID' , 
	APRR# IS 'TRANSACTION         RECORD#' , 
	APSEC2 IS 'BY' , 
	APHSP# IS 'HOSP                #' , 
	APPREC IS 'KEY OF PAT.         MAINT TX' , 
	APACCT IS 'ACCT.#' , 
	APMRC# IS 'MEDICAL RECORD#' , 
	APGAR# IS 'GUARANTOR #' , 
	APPGAR IS 'YRS/GUAR' , 
	APGLNM IS 'LAST NAME' , 
	APGFNM IS 'FIRST NAME' , 
	APGAD1 IS 'ADDRESS LINE 1' , 
	APGAD1E1 IS 'ADDRESS LINE 1 EXTENDED' , 
	APGAD1E2 IS 'ADDRESS LINE 2 EXTENDED' , 
	APGAD2 IS 'ADDRESS LINE 2' , 
	APGCIT IS 'GUARANTOR           CITY' , 
	APGZIP IS 'ZIP' , 
	APGCNT IS 'COUNTY' , 
	APGACD IS 'PHONE AREA CODE' , 
	APGPH# IS 'PHONE NUMBER' , 
	APENM IS 'EMPLOYER''S NAME' , 
	APEADR IS 'EMPLOYR ADDRESS' , 
	APECIT IS 'EMPLOYER            CITY' , 
	APESTE IS 'GUAR EMPL           STATE CODE' , 
	APEZIP IS 'EMPLOYER''S ZIP' , 
	APEZP4 IS 'GUAR EMPL           ZIP EXT.' , 
	APEACD IS 'EMPLOYR AREA-CD' , 
	APEPH# IS 'EMPLOYER PHONE' , 
	APGOCC IS 'GUARANTOR OCCUP' , 
	APFR01 IS 'FIRM NUMBERS        1' , 
	APFR02 IS 'FIRM NUMBERS        2' , 
	APFR03 IS 'FIRM NUMBERS        3' , 
	APGP01 IS 'GROUP # 1' , 
	APGP02 IS 'GROUP # 2' , 
	APGP03 IS 'GROUP # 3' , 
	APPO01 IS 'POLICY              # 1' , 
	APPO02 IS 'POLICY              # 2' , 
	APPO03 IS 'POLICY              # 3' , 
	APSB01 IS 'SUBSCRIBER          NAME 01' , 
	APSB02 IS 'SUBSCRIBER          NAME 02' , 
	APSB03 IS 'SUBSCRIBER          NAME 03' , 
	APRL01 IS 'RELATIONSHIP        01' , 
	APRL02 IS 'RELATIONSHIP        02' , 
	APRL03 IS 'RELATIONSHIP        03' , 
	APNST IS 'NO STMT. FLAG' , 
	APNOTE IS 'A/R NOTES' , 
	APGSTE IS 'GUARANTOR           STATE CODE' , 
	APGZP4 IS 'GUARANTOR           ZIP EXTEN.' , 
	APGCNY IS 'GUARANTOR           COUNTY CODE' , 
	APGSSN IS 'GUARDIANS           SOC. SEC. #' , 
	APGEID IS 'GUARANTOR           EMPLOYEE ID' , 
	APGESC IS 'GUAR EMPLOY         STATUS CODE' , 
	APFR04 IS 'FIRM NUMBERS        1' , 
	APFR05 IS 'FIRM NUMBERS        2' , 
	APFR06 IS 'FIRM NUMBERS        3' , 
	APGP04 IS 'GROUP # 1' , 
	APGP05 IS 'GROUP # 2' , 
	APGP06 IS 'GROUP # 3' , 
	APPO04 IS 'POLICY              # 1' , 
	APPO05 IS 'POLICY              # 2' , 
	APPO06 IS 'POLICY              # 3' , 
	APSB04 IS 'SUBSCRIBER          NAME 01' , 
	APSB05 IS 'SUBSCRIBER          NAME 02' , 
	APSB06 IS 'SUBSCRIBER          NAME 03' , 
	APRL04 IS 'RELATIONSHIP        01' , 
	APRL05 IS 'RELATIONSHIP        02' , 
	APRL06 IS 'RELATIONSHIP        03' , 
	ABGLNM IS 'LAST NAME' , 
	ABGFNM IS 'FIRST NAME' , 
	ABGAD1 IS 'ADDRESS LINE 1' , 
	ABGAD2 IS 'ADDRESS LINE 2' , 
	ABGCIT IS 'GUARANTOR           CITY' , 
	ABGZIP IS 'ZIP' , 
	ABGCNT IS 'COUNTY' , 
	ABGACD IS 'PHONE AREA CODE' , 
	ABGPH# IS 'PHONE NUMBER' , 
	ABENM IS 'EMPLOYER''S NAME' , 
	ABEADR IS 'EMPLOYR ADDRESS' , 
	ABECIT IS 'EMPLOYER            CITY' , 
	ABESTE IS 'GUAR EMPL           STATE CODE' , 
	ABEZIP IS 'EMPLOYER''S ZIP' , 
	ABEZP4 IS 'GUAR EMPL           ZIP EXT.' , 
	ABEACD IS 'EMPLOYR AREA-CD' , 
	ABEPH# IS 'EMPLOYER PHONE' , 
	ABGOCC IS 'GUARANTOR OCCUP' , 
	ABFR01 IS 'FIRM NUMBERS        1' , 
	ABFR02 IS 'FIRM NUMBERS        2' , 
	ABFR03 IS 'FIRM NUMBERS        3' , 
	ABGP01 IS 'GROUP # 1' , 
	ABGP02 IS 'GROUP # 2' , 
	ABGP03 IS 'GROUP # 3' , 
	ABPO01 IS 'POLICY              # 1' , 
	ABPO02 IS 'POLICY              # 2' , 
	ABPO03 IS 'POLICY              # 3' , 
	ABSB01 IS 'SUBSCRIBER          NAME 01' , 
	ABSB02 IS 'SUBSCRIBER          NAME 02' , 
	ABSB03 IS 'SUBSCRIBER          NAME 03' , 
	ABRL01 IS 'RELATIONSHIP        01' , 
	ABRL02 IS 'RELATIONSHIP        02' , 
	ABRL03 IS 'RELATIONSHIP        03' , 
	ABNST IS 'NO STMT. FLAG' , 
	ABNOTE IS 'A/R NOTES' , 
	ABGSTE IS 'GUARANTOR           STATE CODE' , 
	ABGZP4 IS 'GUARANTOR           ZIP EXTEN.' , 
	ABGCNY IS 'GUARANTOR           COUNTY CODE' , 
	ABGSSN IS 'GUARDIANS           SOC. SEC. #' , 
	ABGEID IS 'GUARANTOR           EMPLOYEE ID' , 
	ABGESC IS 'GUAR EMPLOY         STATUS CODE' , 
	ABFR04 IS 'FIRM NUMBERS        1' , 
	ABFR05 IS 'FIRM NUMBERS        2' , 
	ABFR06 IS 'FIRM NUMBERS        3' , 
	ABGP04 IS 'GROUP # 1' , 
	ABGP05 IS 'GROUP # 2' , 
	ABGP06 IS 'GROUP # 3' , 
	ABPO04 IS 'POLICY              # 1' , 
	ABPO05 IS 'POLICY              # 2' , 
	ABPO06 IS 'POLICY              # 3' , 
	ABSB04 IS 'SUBSCRIBER          NAME 01' , 
	ABSB05 IS 'SUBSCRIBER          NAME 02' , 
	ABSB06 IS 'SUBSCRIBER          NAME 03' , 
	ABRL04 IS 'RELATIONSHIP        01' , 
	ABRL05 IS 'RELATIONSHIP        02' , 
	ABRL06 IS 'RELATIONSHIP        03' , 
	APLML IS 'LAST                MAINT               LOG #' , 
	APLMD IS 'LAST                MAINT               DATE' , 
	APLUL# IS 'LAST                UPDATE              LOG #' , 
	APACFL IS 'ADD/CHANGE          FLAG' , 
	APTTME IS 'TIME' , 
	APINLG IS 'INPUT               LOG' , 
	APBYPS IS 'BYPASS              CODE' , 
	APSWPY IS 'SCHEDULER           WORK PTY' , 
	APDRL# IS 'EXT. GAR.           DRIVER LIC#' , 
	APGLOE IS 'GUAR LEN            OF EMPLOY' , 
	APUN IS 'UNION LOCAL         & NUMBER' , 
	ABDRL# IS 'EXT. GAR.           DRIVER LIC#' , 
	ABGLOE IS 'GUAR LEN            OF EMPLOY' , 
	ABUN IS 'UNION LOCAL         & NUMBER' , 
	APGPSM IS 'GUAR SAME           AS INSURED' , 
	ABGPSM IS 'GUAR SAME           AS INSURED' , 
	APGLR IS 'LENGTH RES' , 
	ABGLR IS 'LENGTH RES' , 
	APGLRO IS 'RENT/OWN' , 
	ABGLRO IS 'RENT/OWN' , 
	APTDAT IS 'TRANSACTION         DATE' , 
	APCLRK IS 'A/R                 CLERK' , 
	ABCLRK IS 'A/R                 CLERK' , 
	APZDTE IS 'PROC BY             UPDATER DATE' , 
	APZTME IS 'PROC BY             UPDATER TIME' , 
	APGZPA IS 'GUARANTOR           ZIP' , 
	APGZ4A IS 'GUARANTOR           ZIP+4' , 
	APGCUN IS 'GUARANTOR           COUNTRY' , 
	ABGZPA IS 'GUARANTOR           ZIP' , 
	ABGZ4A IS 'GUARANTOR           ZIP+4' , 
	ABGCUN IS 'GUARANTOR           COUNTRY' , 
	APEZPA IS 'EMPLOYER            ZIP' , 
	APEZ4A IS 'EMPLOYER            ZIP+4' , 
	APECUN IS 'EMPLOYER            COUNTRY' , 
	ABEZPA IS 'EMPLOYER            ZIP' , 
	ABEZ4A IS 'EMPLOYER            ZIP+4' , 
	ABECUN IS 'EMPLOYER            COUNTRY' , 
	ABGMI IS 'Responsible Party   Middle Initial Befor' , 
	APGMI IS 'Responsible Party   Middle Initial After' , 
	ABGSEX IS 'Responsible Party   Gender Code(Before)' , 
	APGSEX IS 'Responsible Party   Gender Code(After)' , 
	APGCPH IS 'GUARANTOR           CELL PHONE' , 
	ABGCPH IS 'GUARANTOR           CELL PHN B4' , 
	APWSIR IS 'WORKSTATION         ID                  REMOTE' , 
	APSECR IS 'SECURITY            ID                  REMOTE' , 
	APORR1 IS 'ORIG                REF1                REMOTE' , 
	APORR2 IS 'ORIG                REF2                REMOTE' , 
	APORR3 IS 'ORIG                REF3                REMOTE' , 
	APGGNRN IS 'PATIENT GENERATION  -AFTER' , 
	ABPGNRN IS 'PATIENT GENERATION  - BFFORE' ) ; 
  
LABEL ON COLUMN PACCESS.PACCAPGM 
( APIDWS TEXT IS 'WORKSTATION ID-ORIG ENTRY' , 
	APIDID TEXT IS 'TRANSACTION FILE ID CODE' , 
	APRR# TEXT IS 'RELATIVE RECORD #-ADT FILE' , 
	APSEC2 TEXT IS 'SECURITY CODE' , 
	APHSP# TEXT IS 'HOSPITAL NUMBER' , 
	APPREC TEXT IS 'KEY OF PATIENT MAINT TRANS' , 
	APACCT TEXT IS 'PATIENT ACCOUNT NUMBER' , 
	APMRC# TEXT IS 'MEDICAL RECORD NUMBER' , 
	APGAR# TEXT IS 'GUARANTOR NUMBER' , 
	APPGAR TEXT IS 'YEARS SINCE LAST ACTIVITY TO' , 
	APGLNM TEXT IS 'GUARANTOR LAST NAME' , 
	APGFNM TEXT IS 'GUARANTOR FIRST NAME' , 
	APGAD1 TEXT IS 'ADDRESS LINE 1' , 
	APGAD1E1 TEXT IS 'ADDRESS LINE 1 EXTENDED' , 
	APGAD1E2 TEXT IS 'ADDRESS LINE 2 EXTENDED' , 
	APGAD2 TEXT IS 'ADDRESS LINE 2' , 
	APGCIT TEXT IS 'GUARANTOR CITY' , 
	APGZIP TEXT IS 'GUARANTOR''S ZIP CODE' , 
	APGCNT TEXT IS 'COUNTY' , 
	APGACD TEXT IS 'AREA CODE' , 
	APGPH# TEXT IS 'PHONE NUMBER' , 
	APENM TEXT IS 'EMPLOYER''S NAME' , 
	APEADR TEXT IS 'EMPLOYER ADDRESS' , 
	APECIT TEXT IS 'EMPLOYER CITY' , 
	APESTE TEXT IS 'GUARANTOR EMPLOYER STATE' , 
	APEZIP TEXT IS 'EMPLOYER''S ZIP CODE' , 
	APEZP4 TEXT IS 'GUARANTOR EMPLOYER ZIP EXT.' , 
	APEACD TEXT IS 'EMPLOYER AREA CODE' , 
	APEPH# TEXT IS 'EMPLOYER PHONE #' , 
	APGOCC TEXT IS 'GUARANTOR''S OCCUPATION/IND' , 
	FLR201 TEXT IS 'RESERVED FILLER # 201' , 
	FLR202 TEXT IS 'RESERVED FILLER # 202' , 
	FLR203 TEXT IS 'RESERVED FILLER # 203' , 
	APFR01 TEXT IS 'FIRM NUMBERS' , 
	APFR02 TEXT IS 'FIRM NUMBERS' , 
	APFR03 TEXT IS 'FIRM NUMBERS' , 
	APGP01 TEXT IS 'GROUP NUMBER - #1' , 
	APGP02 TEXT IS 'GROUP NUMBER - #2' , 
	APGP03 TEXT IS 'GROUP NUMBER - #3' , 
	APPO01 TEXT IS 'POLICY NUMBER - #1' , 
	APPO02 TEXT IS 'POLICY NUMBER - #2' , 
	APPO03 TEXT IS 'POLICY NUMBER - #3' , 
	APSB01 TEXT IS 'SUBSCRIBER NAME - #1' , 
	APSB02 TEXT IS 'SUBSCRIBER NAME - #2' , 
	APSB03 TEXT IS 'SUBSCRIBER NAME - #3' , 
	APRL01 TEXT IS 'RELATIONSHIP - #1' , 
	APRL02 TEXT IS 'RELATIONSHIP - #2' , 
	APRL03 TEXT IS 'RELATIONSHIP - #3' , 
	APNST TEXT IS 'NO STATEMENT FLAG' , 
	APNOTE TEXT IS 'CONTRACT A/R NOTES' , 
	APGSTE TEXT IS 'GUARANTOR STATE CODE' , 
	APGZP4 TEXT IS 'GUARANTOR ZIP CODE EXTENTION' , 
	APGCNY TEXT IS 'GUARANTOR COUNTY CODE' , 
	APGSSN TEXT IS 'GUARDIANS SOCIAL SECURITY #' , 
	APGEID TEXT IS 'GUARANTOR EMPLOYEE ID' , 
	APGESC TEXT IS 'GUARANTOR EMPLOYMENT STATUS' , 
	FLR204 TEXT IS 'RESERVED FILLER # 204' , 
	FLR205 TEXT IS 'RESERVED FILLER # 205' , 
	FLR206 TEXT IS 'RESERVED FILLER # 206' , 
	APFR04 TEXT IS 'FIRM NUMBERS' , 
	APFR05 TEXT IS 'FIRM NUMBERS' , 
	APFR06 TEXT IS 'FIRM NUMBERS' , 
	APGP04 TEXT IS 'GROUP NUMBER - #1' , 
	APGP05 TEXT IS 'GROUP NUMBER - #2' , 
	APGP06 TEXT IS 'GROUP NUMBER - #3' , 
	APPO04 TEXT IS 'POLICY NUMBER - #1' , 
	APPO05 TEXT IS 'POLICY NUMBER - #2' , 
	APPO06 TEXT IS 'POLICY NUMBER - #3' , 
	APSB04 TEXT IS 'SUBSCRIBER NAME - #1' , 
	APSB05 TEXT IS 'SUBSCRIBER NAME - #2' , 
	APSB06 TEXT IS 'SUBSCRIBER NAME - #3' , 
	APRL04 TEXT IS 'RELATIONSHIP - #1' , 
	APRL05 TEXT IS 'RELATIONSHIP - #2' , 
	APRL06 TEXT IS 'RELATIONSHIP - #3' , 
	ABGLNM TEXT IS 'GUARANTOR LAST NAME' , 
	ABGFNM TEXT IS 'GUARANTOR FIRST NAME' , 
	ABGAD1 TEXT IS 'ADDRESS LINE 1' , 
	ABGAD2 TEXT IS 'ADDRESS LINE 2' , 
	ABGCIT TEXT IS 'GUARANTOR CITY' , 
	ABGZIP TEXT IS 'GUARANTOR''S ZIP CODE' , 
	ABGCNT TEXT IS 'COUNTY' , 
	ABGACD TEXT IS 'AREA CODE' , 
	ABGPH# TEXT IS 'PHONE NUMBER' , 
	ABENM TEXT IS 'EMPLOYER''S NAME' , 
	ABEADR TEXT IS 'EMPLOYER ADDRESS' , 
	ABECIT TEXT IS 'EMPLOYER CITY' , 
	ABESTE TEXT IS 'GUARANTOR EMPLOYER STATE' , 
	ABEZIP TEXT IS 'EMPLOYER''S ZIP CODE' , 
	ABEZP4 TEXT IS 'GUARANTOR EMPLOYER ZIP EXT.' , 
	ABEACD TEXT IS 'EMPLOYER AREA CODE' , 
	ABEPH# TEXT IS 'EMPLOYER PHONE #' , 
	ABGOCC TEXT IS 'GUARANTOR''S OCCUPATION/IND' , 
	FLR207 TEXT IS 'RESERVED FILLER # 207' , 
	FLR208 TEXT IS 'RESERVED FILLER # 208' , 
	FLR209 TEXT IS 'RESERVED FILLER # 209' , 
	ABFR01 TEXT IS 'FIRM NUMBERS' , 
	ABFR02 TEXT IS 'FIRM NUMBERS' , 
	ABFR03 TEXT IS 'FIRM NUMBERS' , 
	ABGP01 TEXT IS 'GROUP NUMBER - #1' , 
	ABGP02 TEXT IS 'GROUP NUMBER - #2' , 
	ABGP03 TEXT IS 'GROUP NUMBER - #3' , 
	ABPO01 TEXT IS 'POLICY NUMBER - #1' , 
	ABPO02 TEXT IS 'POLICY NUMBER - #2' , 
	ABPO03 TEXT IS 'POLICY NUMBER - #3' , 
	ABSB01 TEXT IS 'SUBSCRIBER NAME - #1' , 
	ABSB02 TEXT IS 'SUBSCRIBER NAME - #2' , 
	ABSB03 TEXT IS 'SUBSCRIBER NAME - #3' , 
	ABRL01 TEXT IS 'RELATIONSHIP - #1' , 
	ABRL02 TEXT IS 'RELATIONSHIP - #2' , 
	ABRL03 TEXT IS 'RELATIONSHIP - #3' , 
	ABNST TEXT IS 'NO STATEMENT FLAG' , 
	ABNOTE TEXT IS 'CONTRACT A/R NOTES' , 
	ABGSTE TEXT IS 'GUARANTOR STATE CODE' , 
	ABGZP4 TEXT IS 'GUARANTOR ZIP CODE EXTENTION' , 
	ABGCNY TEXT IS 'GUARANTOR COUNTY CODE' , 
	ABGSSN TEXT IS 'GUARDIANS SOCIAL SECURITY #' , 
	ABGEID TEXT IS 'GUARANTOR EMPLOYEE ID' , 
	ABGESC TEXT IS 'GUARANTOR EMPLOYMENT STATUS' , 
	FLR210 TEXT IS 'RESERVED FILLER # 210' , 
	FLR211 TEXT IS 'RESERVED FILLER # 211' , 
	FLR212 TEXT IS 'RESERVED FILLER # 212' , 
	ABFR04 TEXT IS 'FIRM NUMBERS' , 
	ABFR05 TEXT IS 'FIRM NUMBERS' , 
	ABFR06 TEXT IS 'FIRM NUMBERS' , 
	ABGP04 TEXT IS 'GROUP NUMBER - #1' , 
	ABGP05 TEXT IS 'GROUP NUMBER - #2' , 
	ABGP06 TEXT IS 'GROUP NUMBER - #3' , 
	ABPO04 TEXT IS 'POLICY NUMBER - #1' , 
	ABPO05 TEXT IS 'POLICY NUMBER - #2' , 
	ABPO06 TEXT IS 'POLICY NUMBER - #3' , 
	ABSB04 TEXT IS 'SUBSCRIBER NAME - #1' , 
	ABSB05 TEXT IS 'SUBSCRIBER NAME - #2' , 
	ABSB06 TEXT IS 'SUBSCRIBER NAME - #3' , 
	ABRL04 TEXT IS 'RELATIONSHIP - #1' , 
	ABRL05 TEXT IS 'RELATIONSHIP - #2' , 
	ABRL06 TEXT IS 'RELATIONSHIP - #3' , 
	APLML TEXT IS 'LAST MAINTENANCE LOG NUMBER' , 
	APLMD TEXT IS 'DATE OF LAST MAINTENANCE' , 
	APLUL# TEXT IS 'UPDATE LOG NUMBER' , 
	APACFL TEXT IS 'ADD/CHANGE FLAG' , 
	APTTME TEXT IS 'TIME RECORD CREATION' , 
	APINLG TEXT IS 'INPUT LOG NUMBER' , 
	APBYPS TEXT IS 'RECORD BYPASSED BY UPDATE' , 
	APSWPY TEXT IS 'SCHEDULER WORK PRIORITY' , 
	APDRL# TEXT IS 'EXT. GAR. DRIVER LICENSE #' , 
	APGLOE TEXT IS 'GUARANTOR LENGTH OF EMPLOY' , 
	APUN TEXT IS 'UNION LOCAL & NUMBER' , 
	ABDRL# TEXT IS 'EXT. GAR. DRIVER LICENSE #' , 
	ABGLOE TEXT IS 'GUARANTOR LENGTH OF EMPLOY' , 
	ABUN TEXT IS 'UNION LOCAL & NUMBER' , 
	APGPSM TEXT IS 'GUAR SAME AS INSURED' , 
	ABGPSM TEXT IS 'GUAR SAME AS INSURED' , 
	APGLR TEXT IS 'LENGTH RESIDENCE' , 
	ABGLR TEXT IS 'LENGTH RESIDENCE' , 
	APGLRO TEXT IS 'RENT/OWN' , 
	ABGLRO TEXT IS 'RENT/OWN' , 
	APIN01 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APIN02 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APIN03 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APIN04 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APIN05 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APIN06 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN01 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN02 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN03 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN04 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN05 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	ABIN06 TEXT IS 'INSURANCE COMPANY NUMBERS' , 
	APTDAT TEXT IS 'TRANSACTION DATE' , 
	APCLRK TEXT IS 'A/R CLERK' , 
	ABCLRK TEXT IS 'A/R CLERK' , 
	APZDTE TEXT IS 'RECORD PROCESSED BY UPDATER DATE YMD' , 
	APZTME TEXT IS 'RECORD PROCESSED BY UPDATER TIME HH:MM:SS' , 
	APGZPA TEXT IS 'GUARANTOR ZIP' , 
	APGZ4A TEXT IS 'GUARANTOR ZIP+4' , 
	APGCUN TEXT IS 'GUARANTOR COUNTRY' , 
	ABGZPA TEXT IS 'GUARANTOR ZIP' , 
	ABGZ4A TEXT IS 'GUARANTOR ZIP+4' , 
	ABGCUN TEXT IS 'GUARANTOR COUNTRY' , 
	APEZPA TEXT IS 'EMPLOYER ZIP' , 
	APEZ4A TEXT IS 'EMPLOYER ZIP+4' , 
	APECUN TEXT IS 'EMPLOYER COUNTRY' , 
	ABEZPA TEXT IS 'EMPLOYER ZIP' , 
	ABEZ4A TEXT IS 'EMPLOYER ZIP+4' , 
	ABECUN TEXT IS 'EMPLOYER COUNTRY' , 
	APLAST TEXT IS 'LAST TRANSACTION IN GROUP' , 
	APOMR# TEXT IS 'ORIGINAL MR #' , 
	APAPP# TEXT IS 'APPOINTMENT #' , 
	APGEML TEXT IS 'E-MAIL ADDRESS' , 
	ABGEML TEXT IS 'E-MAIL ADDRESS' , 
	ABGMI TEXT IS 'Responsible Party Middle Initial(Before)' , 
	APGMI TEXT IS 'Responsible Party Middle Initial(After)' , 
	ABGSEX TEXT IS 'Responsible Party Gender Code(Before)' , 
	APGSEX TEXT IS 'Responsible Party Gender Code(After)' , 
	APGCPH TEXT IS 'GUARANTOR CELL PHN (AFTER' , 
	ABGCPH TEXT IS 'GUARANTOR CELL PHONE (B4)' , 
	APWSIR TEXT IS 'WORKSTATION ID REMOTE' , 
	APSECR TEXT IS 'SECURITY ID REMOTE' , 
	APORR1 TEXT IS 'ORIG REF 1 REMOTE' , 
	APORR2 TEXT IS 'ORIG REF 2 REMOTE' , 
	APORR3 TEXT IS 'ORIG REF 3 REMOTE' , 
	APGGNRN TEXT IS 'PATIENT GENERATION - AFTER' , 
	ABPGNRN TEXT IS 'PATIENT GENERATION - BFFORE' ) ; 
  
GRANT REFERENCES , UPDATE   
ON PACCESS.PACCAPGM TO HPPGMR WITH GRANT OPTION ; 
  
GRANT UPDATE   
ON PACCESS.PACCAPGM TO PBARDEVL ; 
  
GRANT REFERENCES , UPDATE   
ON PACCESS.PACCAPGM TO PBARSUPP WITH GRANT OPTION ; 
  
GRANT UPDATE   
ON PACCESS.PACCAPGM TO PBARUSER ; 
  
GRANT REFERENCES , UPDATE   
ON PACCESS.PACCAPGM TO QSECOFR WITH GRANT OPTION ; 
  
GRANT ALTER , DELETE , INDEX , INSERT , REFERENCES , SELECT , UPDATE   
ON PACCESS.PACCAPGM TO RMTGRPPRF WITH GRANT OPTION ; 
  
GRANT ALTER , DELETE , INDEX , INSERT , REFERENCES , SELECT , UPDATE   
ON PACCESS.PACCAPGM TO TURNOVER WITH GRANT OPTION ;
