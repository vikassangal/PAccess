/**********************************************************************/
/*                                                                    */
/* iSeries400    SLACGREMDT  - STORED PROCEDURE FOR PX                */
/*                                                                    */
/*    ************************************************************    */
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */
/*    *                                                          *    */
/*    * This unpublished material is proprietary to Perot Sys.   *    */
/*    * The methods and techniques described herein are          *    */
/*    * considered trade secrets and/or confidential.            *    */
/*    * Reproduction or distribution, in whole or in part, is    *    */
/*    * forbidden except by express written permission of        *    */
/*    * Perot Systems, Inc.                                      *    */
/*    ************************************************************    */
/*                                                                    */
/**********************************************************************/
/*  Date        Programmer          Modification Description          */
/* Guarentor Emplotment info                                          */
/**********************************************************************/
/* 02/26/2006   Kevin Sedota        NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTACCTGAREMPDATA                       */
/*    Params     - P_HSP	- an HSP Code		   */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/
--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	02/25/08 09:09:44 
--  Relational Database:       	DVLA 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTACCTGAREMPDATA ( 
	IN P_HSP INTEGER , 
	IN P_MRN INTEGER , 
	IN P_ACCOUNTNUMBER INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC SLACGREMDT 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION DBGVIEW =*SOURCE
	P1 : BEGIN 
  
	DECLARE CURSOR1 CURSOR FOR 
	SELECT 
	LAGAR# AS GUARANTORNUMBER , 
	LAENM AS EMPLOYERSNAME , 
	LAEADR AS EMPLOYERADDRESS , 
	LAECTY AS EMPLOYERCITY , 
	LAEZIP AS EMPLOYERSZIPCODE , 
	LAEACD AS EMPLOYERAREACODE , 
	LAEPH# AS EMPLOYERPHONENUMBER , 
	LAGOCC AS GUARANTORSOCCUPATION , 
	LAHSP# AS HOSPITALNUMBER , 
	LADEL AS DELETECODE , 
	LAECIT AS EMPLOYECITY , 
	LAESTE AS GUARANTOREMPLOYERSTATE , 
	LAEZP4 AS GUARANTOREMPLOYERZIPEXT , 
	LAGESC AS GUARANTOREMPLOYMENTSTATUS , 
	LAGEID AS GUARANTOREMPLOYEEID , 
	LAGLOE AS GUARANTORLENGTHOFEMPLOY , 
	LAUN AS UNIONLOCALANDNUMBER , 
	LAGPSM AS GUARSAMEASINSURED , 
	LAGLR AS LENGTHRESIDENCE , 
	LAGLRO AS RENTOWN , 
	LAEZPA AS EMPLOYERZIP , 
	LAEZ4A AS EMPLOYERZIP4 , 
	LAECUN AS EMPLOYERCOUNTRY 
	FROM HPADLAP1 GE , 
	HPADLPP A 
	WHERE A . LPHSP# = P_HSP 
	AND A . LPACCT = P_ACCOUNTNUMBER 
	AND GE . LAHSP# = A . LPHSP# 
	AND GE . LAGAR# = A . LPGAR# ; 
  
	OPEN CURSOR1 ; 
	 
	END P1  ; 