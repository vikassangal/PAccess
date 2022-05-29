/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLRESEARCHSTUDIES - SQL PROC FOR PX           */        
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
/* 11/10/2009 I Smitha			   I NEW STORED PROCEDURE             */        
/* 01/12/2010 I Deepa Raju		   I Modified Stored procedure name   */        
/*			  I					   I  from SELECTALLRESEARCHSPONSORS  */        
/*			  I					   I  to SELECTALLRESEARCHSTUDIES     */
/*			  I					   I  and added Research sponsor      */
/*			  I					   I  field in SELECT				  */
/* 01/06/2012 I Ahmed Chaudhary	   I  Add field TERMINATIONDATE       */        
/*************I********************I***********************************/       

--  Generate SQL 
--  Version:                   	V6R1M0 080215 
--  Generated on:              	01/20/12 18:11:02 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 for i 
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLRESEARCHSTUDIES ( 
	IN P_FACILITYID INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLALLRSTDY 
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
	P1 : BEGIN 
DECLARE CURSOR1 CURSOR FOR 
SELECT DISTINCT 0 AS OID , 
	TRIM ( QTKEY ) AS CODE , 
	TRIM ( QTRSDC ) AS DESCRIPTION , 
	TRIM ( QTSPON ) AS SPONSOR , 
	TRIM ( QTTRDT ) AS TERMINATIONDATE , 
	QTREGNO AS REGISTRYNUMBER , 
	QTHSP# AS FACILITYID 
FROM HPADQTRZ 
WHERE QTHSP# = P_FACILITYID 
ORDER BY CODE ; 
OPEN CURSOR1 ; 
END P1  ; 
  
