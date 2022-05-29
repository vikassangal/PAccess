/*************I********************************************************/
/*            I                                                       */
/* iSeries400 I EMPLOYERADDRESSFORAPPROVAL - TABLE FOR PATIENT ACCESS */
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
/* 11/21/2008 I SANJEEV             I NEW TABLE CREATION              */
/*************I********************I***********************************/

SET PATH *LIBL;

CREATE TABLE EMPLOYERADDRESSFORAPPROVAL ( 
	ID  INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,	
	EMNEMID DECIMAL(31, 0) NOT NULL ,
	DATETIMEENTERED TIMESTAMP WITH DEFAULT CURRENT TIMESTAMP, 
	EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	EMCODE DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	EMADD# DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	EMADDR CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	EMCITY CHAR(17) CCSID 37 NOT NULL DEFAULT '' , 
	EMST CHAR(2) CCSID 37 NOT NULL DEFAULT '' , 
	EMZIP CHAR(9) CCSID 37 NOT NULL DEFAULT '' , 
	EMPH# DECIMAL(10, 0) NOT NULL DEFAULT 0 , 
	EMADDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMLMDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMANAM CHAR(25) CCSID 37 NOT NULL DEFAULT '' )   
	; 
  
LABEL ON TABLE EMPLOYERADDRESSFORAPPROVAL 
	IS 'Employer address for approval' ; 
  
LABEL ON COLUMN EMPLOYERADDRESSFORAPPROVAL 
( 
	ID     IS 'PRIMARY KEY ID' ,
	EMNEMID IS 'FOREIGN KEY NEW EMP TBLE' ,
	DATETIMEENTERED IS 'DATE TIME ENTRY ENTERED' ,
	EMFUUN IS 'FOLLOW UP UNIT #' , 
	EMCODE IS 'EMPLOYER CODE' , 
	EMADD# IS 'EMPLOYER ADDR#' , 
	EMADDR IS 'EMPLOYER ADDRESS' , 
	EMCITY IS 'EMPLOYER CITY' , 
	EMST IS 'EMPLOYER STATE' , 
	EMZIP IS 'EMPLOYER ZIP' , 
	EMPH# IS 'EMPLOYER PHONE#' , 
	EMADDT IS 'ADD DATE' , 
	EMLMDT IS 'LAST MAINT DATE' , 
	EMANAM IS 'EMPLOYER NAME' ) ; 
  
LABEL ON COLUMN EMPLOYERADDRESSFORAPPROVAL 
( 
	ID     TEXT IS 'PRIMARY KEY ID' ,
	EMNEMID TEXT IS 'FOREIGN KEY NEW EMP TBLE' ,
	DATETIMEENTERED TEXT IS 'DATE TIME ENTRY ENTERED' ,
	EMFUUN TEXT IS 'FOLLOW UP UNIT #' , 
	EMCODE TEXT IS 'EMPLOYER CODE' , 
	EMADD# TEXT IS 'EMPLOYER ADDR#' , 
	EMADDR TEXT IS 'EMPLOYER ADDRESS' , 
	EMCITY TEXT IS 'EMPLOYER CITY' , 
	EMST TEXT IS 'EMPLOYER STATE' , 
	EMZIP TEXT IS 'EMPLOYER ZIP' , 
	EMPH# TEXT IS 'EMPLOYER PHONE#' , 
	EMADDT TEXT IS 'ADD DATE' , 
	EMLMDT TEXT IS 'LAST MAINT DATE' , 
	EMANAM TEXT IS 'EMPLOYER NAME' ) ; 
  
 