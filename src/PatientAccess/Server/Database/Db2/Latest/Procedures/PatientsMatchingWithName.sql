/**********************************************************************/        
/*  AS400 Short Name: PATSMATCHN                                      */        
/*  iSeries400        PATIENTSMATCHINGWITHNAME -SQL PROC              */        
/*                                                                    */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *                                                          *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods AND techniques described herein are          *    */        
/*    * considered trade secrets AND/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*                                                                    */        
/*                                                                    */        
/**************I*****************I*************************************/        
/*  Date       I Programmer      I    Modification Description        */        
/**************I*****************I*************************************/        
/* 02/28/2008  I Kevin Sedota    I  New Stored Procedure              */       
/* 03/18/2008  I Kevin Sedota    I  Use search names                  */       
/* 03/19/2008  I Kevin Sedota    I  remove un-needed search           */       
/* 05/08/2008  I Jithin		     I  remove temp tables	              */       
/* 05/14/2008  I JITHIN          I  PERFORMANCE TUNING                */
/* 07/23/2008  I Deepa Raju      I  OTD# 37801 fix - Do not set AKA   */
/*             I                 I  Entry Date to MinValue            */
/**********************************************************************/        
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE PATIENTSMATCHINGWITHNAME ( 
	IN @P_FACILITYID INTEGER , 
	IN @P_FNAME VARCHAR(25) , 
	IN @P_LNAME VARCHAR(25) , 
	IN @P_HSPCODE VARCHAR(3) ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PATSMATCHN
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DBGVIEW = *SOURCE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	P1 : BEGIN 
  
DECLARE ROW_COUNT INTEGER DEFAULT 0 ; 
  
DECLARE CURSOR1 CURSOR FOR		 
		 
	 
		 -- PULL ALL ALIASES 
		 
						SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				1 AS TYPEOFNAMEID , 
				TRIM ( DEM . MDPFNM ) AS FIRSTNAME , 
				TRIM ( DEM . MDPLNM ) AS LASTNAME , 
				2 AS DISPLAYTYPEOFNAMEID , 
				TRIM ( DEM . MDPMI ) AS MIDDLEINITIAL ,	 
				TRIM ( AKA . AKPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( AKA . AKPLNM ) AS DISPLAYLASTNAME , 
				'' AS DISPLAYMIDDLEINITIAL ,		 
			 
															 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
					 
				6 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'PHYSICAL' AS TYPEOFCONTACTPOINT , 
				DEM . MDMADR AS ADDRESS1 , 
				'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDMCIT AS CITY , 
				RTRIM ( DEM . MDMZPA ) || RTRIM ( DEM . MDMZ4A ) AS POSTALCODE , 
				0 AS STATEID , 
				DEM . MDMSTE AS STATECODE , 
				TRIM ( '' ) AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID , 
				'' AS COUNTRYCODE , 
				'' AS COUNTRYDESCRIPTION 
				 
				 
				FROM HPADMDP DEM	 
					 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
					 
				LEFT OUTER JOIN NMNHAKAP AKA2	 
				ON AKA2 . AKHSP# = AKA . AKHSP# 
				AND	AKA2 . AKMRC# = AKA . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND	( AKA2 . AKPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND	( AKA2 . AKPLSN LIKE UPPER ( @P_LNAME ) || '%' ) 
				 
					 
	UNION 
  
SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				1 AS TYPEOFNAMEID , 
				TRIM ( DEM . MDPFNM ) AS FIRSTNAME , 
				TRIM ( DEM . MDPLNM ) AS LASTNAME , 
				2 AS DISPLAYTYPEOFNAMEID , 
				TRIM ( DEM . MDPMI ) AS MIDDLEINITIAL ,												 
				 
				TRIM ( AKA . AKPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( AKA . AKPLNM ) DISPLAYLASTNAME , 
				'' AS DISPLAYMIDDLEINITIAL ,		 
			 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
				 
				0 AS CONTACTPOINTTYPEID2 ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'MAILING' AS TYPEOFCONTACTPOINT2 , 
				DEM . MDPADR AS ADDRESS12 , 
				'' AS ADDRESS22 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDPCIT AS CITY2 , 
				RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE2 , 
				0 AS STATEID2 , 
				DEM . MDPSTE AS STATECODE2 , 
				TRIM ( '' ) AS STATEDESCRIPTION2 ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID2 , 
				'' AS COUNTRYCODE2 , 
				'' ASCOUNTRYDESCRIPTION2 
				 
				FROM HPADMDP DEM	 
					 
				 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
				LEFT OUTER JOIN NMNHAKAP AKA2	 
				ON AKA2 . AKHSP# = AKA . AKHSP# 
				AND	AKA2 . AKMRC# = AKA . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND	( AKA . AKPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND	( AKA . AKPLSN LIKE UPPER ( @P_LNAME ) || '%' ) 
			 
	UNION 
  
SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				2 AS TYPEOFNAMEID , 
					TRIM ( AKA . AKPFNM ) AS FIRSTNAME , 
				TRIM ( AKA . AKPLNM ) AS LASTNAME , 
				1 AS DISPLAYTYPEOFNAMEID , 
				'' AS MIDDLEINITIAL ,	 
				 
				TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
				TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
														 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
				 
				0 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'MAILING' AS TYPEOFCONTACTPOINT , 
				DEM . MDPADR AS ADDRESS1 , 
				'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDPCIT AS CITY , 
				RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
				0 AS STATEID , 
				DEM . MDPSTE AS STATECODE , 
				TRIM ( '' ) AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID , 
				'' AS COUNTRYCODE , 
				'' AS COUNTRYDESCRIPTION 
				 
				FROM HPADMDP DEM	 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
					 
				WHERE DEM . MDHSP# = @P_FACILITYID						 
				AND ( DEM . MDPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND ( DEM . MDPLSN LIKE UPPER ( @P_LNAME ) || '%' ) 
				 
  
UNION 
  
SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				2 AS TYPEOFNAMEID , 
				TRIM ( AKA . AKPFNM ) AS FIRSTNAME , 
				TRIM ( AKA . AKPLNM ) AS LASTNAME , 
				1 AS DISPLAYTYPEOFNAMEID , 
				'' AS MIDDLEINITIAL ,												 
				 
				TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
				TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
				 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
				 
				6 AS CONTACTPOINTTYPEID2 ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'PHYSICAL' AS TYPEOFCONTACTPOINT2 , 
				DEM . MDMADR AS ADDRESS12 , 
				'' AS ADDRESS22 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDMCIT AS CITY2 , 
				RTRIM ( DEM . MDMZPA ) || RTRIM ( DEM . MDMZ4A ) AS POSTALCODE2 , 
				0 AS STATEID2 , 
				DEM . MDMSTE AS STATECODE2 , 
				TRIM ( '' ) AS STATEDESCRIPTION2 ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID2 , 
				'' AS COUNTRYCODE2 , 
				'' AS COUNTRYDESCRIPTION2 
				FROM HPADMDP DEM	 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
					 
				WHERE DEM . MDHSP# = @P_FACILITYID						 
				AND ( DEM . MDPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND ( DEM . MDPLSN LIKE UPPER ( @P_LNAME ) || '%' ) ;
  
  
				/*UNION 
				 
				SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				1 AS TYPEOFNAMEID , 
				TRIM ( AKAJ . AKPFNM ) AS FIRSTNAME , 
				TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
				2 AS DISPLAYTYPEOFNAMEID , 
				'' AS MIDDLEINITIAL ,												 
				 
				TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
				'' AS DISPLAYMIDDLEINITIAL ,		 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
				 
				0 AS TYPEOFCONTACTPOINT ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'MAILING' AS TYPEOFCONTACTPOINT , 
				DEM . MDPADR AS ADDRESS1 , 
				'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDPCIT AS CITY , 
				RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
				0 AS STATEID , 
				DEM . MDPSTE AS STATECODE , 
				TRIM ( '' ) AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID , 
				'' AS COUNTRYCODE , 
				'' AS COUNTRYDESCRIPTION 
				 
				 
						 
				FROM HPADMDP DEM 
				 
				LEFT OUTER JOIN NMNHAKAP AKAJ		 
				ON DEM . MDHSP#	= AKAJ . AKHSP#	 
				AND DEM . MDMRC#	= AKAJ . AKMRC# 
				LEFT OUTER JOIN NMNHAKAP AKASJ		 
				ON AKAJ . AKHSP#	= AKASJ . AKHSP#	 
				AND AKAJ . AKMRC#	= AKASJ . AKMRC# 
				 
			LEFT OUTER JOIN NMNHAKAP AKA3	 
				ON AKA3 . AKHSP# = AKAJ . AKHSP# 
				AND AKA3 . AKMRC# = AKAJ . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND	( AKAJ . AKPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND	( AKAJ . AKPLSN LIKE UPPER ( @P_LNAME ) || '%' )	 
				 
			UNION 
			 
			SELECT	DISTINCT 
				DEM . MDHSP# AS FACILITYID , 
				0 AS PERSONID , 
				AKA . AKEDAT AS ENTRYDATE , 
				DEM . MDSSN9 AS SSN , 
				@P_HSPCODE AS HSPCODE , 
				DEM . MDSEX AS SEXCODE , 
				DEM . MDMRC# AS MRN , 
				DEM . MDDOB8 AS DOB , 
				1 AS TYPEOFNAMEID , 
				TRIM ( AKAJ . AKPFNM ) AS FIRSTNAME , 
				TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
				2 AS DISPLAYTYPEOFNAMEID , 
				'' AS MIDDLEINITIAL ,												 
				 
				TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
				TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
				'' AS DISPLAYMIDDLEINITIAL ,		 
				DEM . MDNMTL AS TITLE , 
				AKA . AKSECF AS CONFIDENTIALFLAG ,  -- SECURITY FLAG 
				 
				6 AS CONTACTPOINTTYPEID2 ,  -- PHYSICAL/PERMANENT CONTACT POINT 
				'PHYSICAL' AS TYPEOFCONTACTPOINT2 , 
				DEM . MDMADR AS ADDRESS12 , 
				'' AS ADDRESS22 ,  -- ADDRESS 2 IS BLANK                               
				DEM . MDMCIT AS CITY2 , 
				RTRIM ( DEM . MDMZPA ) || RTRIM ( DEM . MDMZ4A ) AS POSTALCODE2 , 
				0 AS STATEID2 , 
				DEM . MDMSTE AS STATECODE2 , 
				TRIM ( '' ) AS STATEDESCRIPTION2 ,  -- STATE DESCRIPTION 
				0 AS COUNTRYID2 , 
				'' AS COUNTRYCODE2 , 
				'' AS COUNTRYDESCRIPTION2 
						 
				FROM HPADMDP DEM 
				 
				LEFT OUTER JOIN NMNHAKAP AKAJ		 
				ON DEM . MDHSP#	= AKAJ . AKHSP#	 
				AND DEM . MDMRC#	= AKAJ . AKMRC# 
				LEFT OUTER JOIN NMNHAKAP AKASJ		 
				ON AKAJ . AKHSP#	= AKASJ . AKHSP#	 
				AND AKAJ . AKMRC#	= AKASJ . AKMRC# 
				 
				LEFT OUTER JOIN NMNHAKAP AKA3	 
				ON AKA3 . AKHSP# = AKAJ . AKHSP# 
				AND AKA3 . AKMRC# = AKAJ . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND	( AKASJ . AKPFSN LIKE UPPER ( @P_FNAME ) || '%' ) 
				AND	( AKASJ . AKPLSN LIKE UPPER ( @P_LNAME ) || '%' )	*/								 
								 
		
	 
  
OPEN CURSOR1 ; 
	 
END P1  ;
