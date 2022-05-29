 /**********************************************************************/        
/* AS400 Short Name: PATSMATCHA                                       */        
/* iSeries400        PATIENTSMATCHINGWITHACCT -SQL PROC               */        
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
/**************I*******************I***********************************/        
/*  Date       I Programmer        I  Modification Description        */        
/**************I*******************I***********************************/        
/* 02/28/2008  I Kevin Sedota      I   New Stored Procedure           */  
/* 05/08/2008  I Jithin		       I   remove temp tables            */  
/* 05/20/2008  I JITHIN            I PERFORMANCE TUNING               */
/**********************************************************************/    

SET PATH *LIBL ; 
  
CREATE PROCEDURE PATIENTSMATCHINGWITHACCT ( 
	IN @P_FACILITYID INTEGER , 
	IN @P_ACCOUNTNUMBER INTEGER , 
	IN @P_HSPCODE VARCHAR(3) ) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PATSMATCHA 
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
		 
		 
			SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKA.AKEDAT					AS ENTRYDATE,	  
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					2 TYPEOFNAMEID , 
					TRIM ( AKA . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKA . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					1 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
					TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					0 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'MAILING' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
  
								 
				FROM HPADLPP PAT 
				LEFT OUTER JOIN HPADMDP DEM	 
				ON PAT . LPHSP# = DEM . MDHSP# 
				AND PAT . LPMRC# = DEM . MDMRC# 
							 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
						 
				WHERE	PAT . LPHSP# = @P_FACILITYID 
				AND	PAT . LPACCT = @P_ACCOUNTNUMBER 
				 
				 
				UNION 
				 
				SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKA.AKEDAT					AS ENTRYDATE,	  
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					2 TYPEOFNAMEID , 
					TRIM ( AKA . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKA . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					1 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
					TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					6 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'PHYSICAL' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
  
								 
				FROM HPADLPP PAT 
				LEFT OUTER JOIN HPADMDP DEM	 
				ON PAT . LPHSP# = DEM . MDHSP# 
				AND PAT . LPMRC# = DEM . MDMRC# 
							 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
						 
				WHERE	PAT . LPHSP# = @P_FACILITYID 
				AND	PAT . LPACCT = @P_ACCOUNTNUMBER 
				 
	/***********************************************************/ 
	UNION 
				 
				 -- PULL ALIASES 
				 
			 
			SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKAJ.AKEDAT					AS ENTRYDATE,	  
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					1 TYPEOFNAMEID , 
					TRIM ( AKAJ . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					2 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
					'' AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					0 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'MAILING' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
							 
				FROM HPADLPP PAT 
				LEFT OUTER JOIN HPADMDP DEM	 
				ON PAT . LPHSP# = DEM . MDHSP# 
				AND PAT . LPMRC# = DEM . MDMRC# 
					 
				LEFT OUTER JOIN NMNHAKAP AKAJ		 
				ON DEM . MDHSP#	= AKAJ . AKHSP#	 
				AND DEM . MDMRC#	= AKAJ . AKMRC# 
					 
				LEFT OUTER JOIN NMNHAKAP AKASJ		 
				ON AKAJ . AKHSP#	= AKASJ . AKHSP#	 
				AND AKAJ . AKMRC#	= AKASJ . AKMRC# 
			 
				LEFT OUTER JOIN NMNHAKAP AKA3	 
				ON AKA3 . AKMRC# = AKAJ . AKMRC# 
				WHERE PAT . LPHSP# = @P_FACILITYID 
				AND	AKA3 . AKHSP# = @P_FACILITYID										 
				AND	( PAT . LPACCT = @P_ACCOUNTNUMBER )	 
				 
				UNION 
				 
				SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKAJ.AKEDAT					AS ENTRYDATE,	 
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					1 TYPEOFNAMEID , 
					TRIM ( AKAJ . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					2 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
					'' AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					6 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'PHYSICAL' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
							 
				FROM HPADLPP PAT 
				LEFT OUTER JOIN HPADMDP DEM	 
				ON PAT . LPHSP# = DEM . MDHSP# 
				AND PAT . LPMRC# = DEM . MDMRC# 
					 
				LEFT OUTER JOIN NMNHAKAP AKAJ		 
				ON DEM . MDHSP#	= AKAJ . AKHSP#	 
				AND DEM . MDMRC#	= AKAJ . AKMRC# 
					 
				LEFT OUTER JOIN NMNHAKAP AKASJ		 
				ON AKAJ . AKHSP#	= AKASJ . AKHSP#	 
				AND AKAJ . AKMRC#	= AKASJ . AKMRC# 
			 
				LEFT OUTER JOIN NMNHAKAP AKA3	 
				ON AKA3 . AKMRC# = AKAJ . AKMRC# 
				WHERE PAT . LPHSP# = @P_FACILITYID 
				AND	AKA3 . AKHSP# = @P_FACILITYID										 
				AND	( PAT . LPACCT = @P_ACCOUNTNUMBER )	 
/***********************************************************************/					 
  
				UNION 
	 
				 -- PULL DEMOGRAPHICS-ONLY PATIENTS 
							 
			SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKA.AKEDAT					AS ENTRYDATE,	 
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					2 TYPEOFNAMEID , 
					TRIM ( AKA . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKA . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					1 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
					TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					0 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'MAILING' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
					 
				FROM HPADMDP DEM 
				 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
				 
				WHERE	DEM . MDHSP# = @P_FACILITYID 
				AND	( DEM . MDACCT = @P_ACCOUNTNUMBER )	 
				 
				 
				UNION 
				 
				SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKA.AKEDAT					AS ENTRYDATE,	 
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					2 TYPEOFNAMEID , 
					TRIM ( AKA . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKA . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					1 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( DEM . MDPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( DEM . MDPLNM ) AS DISPLAYLASTNAME , 
					TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					6 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'PHYSICAL' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
					0 AS COUNTRYID , 
					'' AS COUNTRYCODE , 
					'' AS COUNTRYDESCRIPTION 
					 
				FROM HPADMDP DEM 
				 
				LEFT OUTER JOIN NMNHAKAP AKA 
				ON DEM . MDHSP# = AKA . AKHSP# 
				AND DEM . MDMRC# = AKA . AKMRC# 
				 
				WHERE	DEM . MDHSP# = @P_FACILITYID 
				AND	( DEM . MDACCT = @P_ACCOUNTNUMBER )	 
				 
	/*******************************************************************/		 
			 
				 -- PULL ALIASES FOR DEM-ONLY 
					UNION 
								 
				SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKAJ.AKEDAT					AS ENTRYDATE,	 
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					1 TYPEOFNAMEID , 
					TRIM ( AKAJ . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					2 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
					'' AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					0 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'MAILING' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
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
				ON AKAJ . AKMRC# = AKA3 . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND AKA3 . AKHSP# = @P_FACILITYID	 
				AND ( DEM . MDACCT = @P_ACCOUNTNUMBER )		 
				 
				 
				UNION 
				 
				SELECT	DISTINCT 
					DEM . MDHSP# AS FACILITYID , 
					0 AS PERSONID , 
					AKAJ.AKEDAT					AS ENTRYDATE,	 
					DEM . MDSSN9 AS SSN , 
					@P_HSPCODE AS HSPCODE , 
					DEM . MDSEX AS SEXCODE , 
					DEM . MDMRC# AS MRN , 
					DEM . MDDOB8 AS DOB , 
					1 TYPEOFNAMEID , 
					TRIM ( AKAJ . AKPFNM ) AS	FIRSTNAME , 
					TRIM ( AKAJ . AKPLNM ) AS LASTNAME , 
					'' AS MIDDLEINITIAL ,	 
					2 AS DISPLAYTYPEOFNAMEID ,	 
					TRIM ( AKASJ . AKPFNM ) AS DISPLAYFIRSTNAME , 
					TRIM ( AKASJ . AKPLNM ) AS DISPLAYLASTNAME , 
					TRIM ( DEM . MDPMI ) AS DISPLAYMIDDLEINITIAL ,		 
					DEM . MDNMTL AS TITLE , 
					'' AS CONFIDENTIALFLAG ,  -- MPI.RWCNFG	 
					6 AS CONTACTPOINTTYPEID ,  -- PHYSICAL/PERMANENT CONTACT POINT 
					'PHYSICAL' AS TYPEOFCONTACTPOINT , 
					DEM . MDPADR AS ADDRESS1 , 
					'' AS ADDRESS2 ,  -- ADDRESS 2 IS BLANK                               
					DEM . MDPCIT AS CITY , 
					RTRIM ( DEM . MDPZPA ) || RTRIM ( DEM . MDPZ4A ) AS POSTALCODE , 
					0 AS STATEID , 
					DEM . MDPSTE AS STATECODE , 
					'' AS STATEDESCRIPTION ,  -- STATE DESCRIPTION 
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
				ON AKAJ . AKMRC# = AKA3 . AKMRC# 
				WHERE DEM . MDHSP# = @P_FACILITYID 
				AND AKA3 . AKHSP# = @P_FACILITYID	 
				AND ( DEM . MDACCT = @P_ACCOUNTNUMBER )	 
				 
				ORDER BY	 
			MRN , 
			DISPLAYTYPEOFNAMEID ASC , 
			LASTNAME , 
			FIRSTNAME , 
			MIDDLEINITIAL , 
			ENTRYDATE DESC 
			FOR	READ ONLY 
		OPTIMIZE FOR 10 ROWS ;	 
				 
	 
OPEN CURSOR1 ; 
	 
END P1  ;
