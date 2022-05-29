                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETPREREGPREGNANCYINDICATOR - SQL PROC FOR PX        */        
/*            I                                                       */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2010, All rights reserved(U.S.) *    */        
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
/* 04/09/2010 I Smitha			   I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ; 
  
CREATE PROCEDURE GETPREREGPREGNANCYINDICATOR ( 
	IN @P_FACILITYID INTEGER , 
	IN @P_MRN INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC GETPRGPI 
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
DECLARE CURSOR1 CURSOR FOR 
				SELECT 
				RRN ( PI ) AS ROWNUMBER , 
					PI . TMPRGI AS ISPREGNANT 
				FROM 
					 HPTMPADP PI  -- TEMP PI TABLE 
				WHERE 
					PI . TMHSP# = @P_FACILITYID 
				AND PI . TMMRC# = @P_MRN 
				AND PI . TMACCT = P_ACCOUNTNUMBER 
				ORDER BY ROWNUMBER DESC 
				FETCH FIRST ROW ONLY ; 
OPEN CURSOR1 ; 
END P1  ;
