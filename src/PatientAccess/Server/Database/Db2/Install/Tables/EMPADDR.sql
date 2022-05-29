--  Generate SQL 
-- *****************************************************************************/
-- * PACCESS.EMPADDR                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: kevin sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/01/2004                                                      */
-- *****************************************************************************/  

CREATE TABLE PACCESS.EMPADDR ( 
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
	EMANAM CHAR(25) CCSID 37 NOT NULL DEFAULT '',
	TXNDATE TIMESTAMP ) ; 

LABEL ON TABLE PACCESS.EMPADDR 
	IS 'Employer address' ; 

LABEL ON COLUMN PACCESS.EMPADDR 
( EMFUUN IS 'FOLLOW UP           UNIT #' , 
	EMCODE IS 'EMPLOYER CODE' , 
	EMADD# IS 'EMPLOYER ADDR#' , 
	EMADDR IS 'EMPLOYER ADDRESS' , 
	EMCITY IS 'EMPLOYER CITY' , 
	EMST IS 'EMPLOYER STATE' , 
	EMZIP IS 'EMPLOYER ZIP' , 
	EMPH# IS 'EMPLOYER PHONE#' , 
	EMADDT IS 'ADD DATE' , 
	EMLMDT IS 'LAST MAINT          DATE' , 
	EMANAM IS 'EMPLOYER NAME',
	TXNDATE IS 'TXN TIMESTAMP' ) ; 

  
LABEL ON COLUMN PACCESS.EMPADDR 
( EMFUUN TEXT IS 'FOLLOW UP UNIT #' , 
	EMCODE TEXT IS 'EMPLOYER CODE' , 
	EMADD# TEXT IS 'EMPLOYER ADDR#' , 
	EMADDR TEXT IS 'EMPLOYER ADDRESS' , 
	EMCITY TEXT IS 'EMPLOYER CITY' , 
	EMST TEXT IS 'EMPLOYER STATE' , 
	EMZIP TEXT IS 'EMPLOYER ZIP' , 
	EMPH# TEXT IS 'EMPLOYER PHONE#' , 
	EMADDT TEXT IS 'ADD DATE' , 
	EMLMDT TEXT IS 'LAST MAINT DATE' , 
	EMANAM TEXT IS 'EMPLOYER NAME',
	TXNDATE TEXT IS 'TXN TIMESTAMP' ) ;
 