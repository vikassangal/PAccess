-- *****************************************************************************/
-- * PACCESS.EMPTXN                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: kevin sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/01/2004                                                      */
-- *****************************************************************************/
CREATE TABLE PACCESS.EMPTXN ( 

	EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	EMNAME CHAR(25) CCSID 37 NOT NULL DEFAULT '' , 
	EMURFG CHAR(1) CCSID 37 NOT NULL DEFAULT '' , 
	EMADDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMLMDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMDLDT DECIMAL(6, 0) NOT NULL DEFAULT 0 , 
	EMCODE DECIMAL(7, 0) NOT NULL DEFAULT 0 , 
	EMACNT DECIMAL(5, 0) NOT NULL DEFAULT 0 , 
	EMUSER CHAR(10) CCSID 37 NOT NULL DEFAULT '' , 
	EMNEID CHAR(10) CCSID 37 NOT NULL DEFAULT '' ,
	TXNDATE TimeStamp)	 ; 

LABEL ON TABLE PACCESS.EMPTXN 
	IS 'PaccessEmployer' ; 
 