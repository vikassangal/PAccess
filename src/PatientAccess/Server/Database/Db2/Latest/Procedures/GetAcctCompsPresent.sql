/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETACCMPR            - SQL PROC FOR PATIENT ACCESS   */        
/*            I                                                       */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *       I                                                  *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods and techniques described herein are          *    */        
/*    * considered trade secrets and/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*            I                                                       */        
/*************I********************I***********************************/        
/*  Date      I  Programmer        I  Modification Description        */        
/*************I********************I***********************************/        
/* 04/24/2008 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETACCTCOMPSPRESENT ( 
	IN P_HSP INTEGER , 
	IN P_HSPCODE CHAR(3) , 
	IN P_MRN INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER , 
	OUT O_EMERGCONTACTPRESENT INTEGER , 
	OUT O_NEARESTRELAVIVEPRESENT INTEGER , 
	OUT O_MSPPRESENT INTEGER , 
	OUT O_FINDATAPRESENT INTEGER , 
	OUT O_BENIFITVALPRESENT INTEGER ) 
	LANGUAGE SQL 
	SPECIFIC GETACCMPR 
	NOT DETERMINISTIC 
	READS SQL DATA 
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
	BEGIN 
  
	DECLARE MCD_CNT INTEGER ; 
	DECLARE WKC_CNT INTEGER ; 
	DECLARE MCR_CNT INTEGER ; 
	DECLARE GOV_CNT INTEGER ; 
	DECLARE SLF_CNT INTEGER ; 
	DECLARE COM_CNT INTEGER ; 
	DECLARE CBC_CNT INTEGER ; 
  
	SELECT COUNT ( * ) INTO O_EMERGCONTACTPRESENT 
	FROM HPADLAP3 
	WHERE LAHSP# = P_HSP 
	AND LAMRC# = P_MRN; 
  
	SELECT COUNT ( * ) INTO O_NEARESTRELAVIVEPRESENT 
	FROM HPADLAP2 
	WHERE LAHSP# = P_HSP 
	AND LAMRC# = P_MRN; 
  
	SELECT COUNT ( * ) INTO O_MSPPRESENT 
	FROM NHMSP01P 
	WHERE MSPHSP# = P_HSP 
	AND MSPMRC# = P_MRN 
	AND MSPPT#9 = P_ACCOUNTNUMBER; 
  
	SELECT COUNT ( * ) INTO O_FINDATAPRESENT 
	FROM PM0001P P 
	WHERE PMHSPC = P_HSPCODE 
	AND PMPT#9 = P_ACCOUNTNUMBER ; 
	 
	SELECT COUNT ( * ) INTO MCD_CNT 
	FROM NHADMCDCP 
	WHERE MCDCHSP# = P_HSP 
	AND MCDCACCT = P_ACCOUNTNUMBER ; 
	 
	SELECT COUNT ( * ) INTO WKC_CNT 
	FROM NHADWKCCP 
	WHERE WKCCHSP# = P_HSP 
	AND WKCCACCT = P_ACCOUNTNUMBER ; 
	 
	SELECT COUNT ( * ) INTO MCR_CNT 
	FROM NHADMCRCP 
	WHERE MCRCHSP# = P_HSP 
	AND MCRCACCT = P_ACCOUNTNUMBER ; 
	 
	SELECT COUNT ( * ) INTO GOV_CNT 
	FROM NHADGOVCP 
	WHERE GOVCHSP# = P_HSP 
	AND GOVCACCT = P_ACCOUNTNUMBER ; 
	 
	SELECT COUNT ( * ) INTO SLF_CNT 
	FROM NHADSLFCP 
	WHERE SLFCHSP# = P_HSP 
	AND SLFCACCT = P_ACCOUNTNUMBER ; 
  
	SELECT COUNT ( * ) INTO COM_CNT 
	FROM NHADCOMCP 
	WHERE COMCHSP# = P_HSP 
	AND COMCACCT = P_ACCOUNTNUMBER ; 
  
	SELECT COUNT ( * ) INTO CBC_CNT 
	FROM NHADCBCP 
	WHERE CBCHSP# = P_HSP 
	AND CBCACCT = P_ACCOUNTNUMBER ; 
	 
	SET O_BENIFITVALPRESENT = MCD_CNT + WKC_CNT + MCR_CNT + GOV_CNT + SLF_CNT + COM_CNT + CBC_CNT ; 
END  ;
