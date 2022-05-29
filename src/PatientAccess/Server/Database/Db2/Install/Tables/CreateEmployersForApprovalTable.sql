/*************I********************************************************/
/*            I                                                       */
/* iSeries400 I  EMPLOYERSFORAPPROVAL - TABLE FOR PATIENT ACCESS      */
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

CREATE TABLE EMPLOYERSFORAPPROVAL ( 
	ID  INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	DATETIMEENTERED TIMESTAMP WITH DEFAULT CURRENT TIMESTAMP, 
	EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	EMNAME CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	EMURFG CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	EMADDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMLMDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMDLDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMCODE DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	EMACNT DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	EMUSER VARCHAR(255) CCSID 37 NOT NULL DEFAULT '' , 
	EMNEID CHAR(10) CCSID 37 NOT NULL DEFAULT '' )   
	; 
  
LABEL ON TABLE EMPLOYERSFORAPPROVAL 
	IS 'EmployerForApproval' ; 
  
LABEL ON COLUMN EMPLOYERSFORAPPROVAL 
( 
	ID     IS ' PRIMARY KEY ID' ,
	DATETIMEENTERED IS 'DATE TIME ENTRY ENTERED' ,
	EMFUUN IS 'FOLLOW UP UNIT #' , 
	EMNAME IS 'EMPLOYER NAME' , 
	EMURFG IS 'U/R FLAG' , 
	EMADDT IS 'ADD DATE' , 
	EMLMDT IS 'LAST MAINT DATE' , 
	EMDLDT IS 'DELETE DATE' , 
	EMCODE IS 'EMPLOYER CODE' , 
	EMACNT IS 'ADDRESS COUNT' , 
	EMUSER IS 'ADDED BY' , 
	EMNEID IS 'NATIONAL EMPLOYER ID' ) ; 
  
LABEL ON COLUMN EMPLOYERSFORAPPROVAL 
( 
	ID 	   TEXT IS ' PRIMARY KEY ID' ,
	DATETIMEENTERED TEXT IS 'DATE TIME ENTRY ENTERED' ,
	EMFUUN TEXT IS 'FOLLOW UP UNIT #' , 
	EMNAME TEXT IS 'EMPLOYER NAME' , 
	EMURFG TEXT IS 'U/R FLAG' , 
	EMADDT TEXT IS 'ADD DATE' , 
	EMLMDT TEXT IS 'LAST MAINT DATE' , 
	EMDLDT TEXT IS 'DELETE DATE' , 
	EMCODE TEXT IS 'EMPLOYER CODE' , 
	EMACNT TEXT IS 'ADDRESS COUNT' , 
	EMUSER TEXT IS 'ADDED BY' , 
	EMNEID TEXT IS 'NATIONAL EMPLOYER ID' ) ; 
   
RENAME EMPLOYERSFORAPPROVAL TO SYSTEM NAME EMPFORAPRL;    