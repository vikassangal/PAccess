 /*************I********************************************************/
/*            I                                                       */
/* iSeries400 I  TENTPLANID  - TABLE FOR PATIENT ACCESS               */
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
/* 05/30/2008 I SANJEEV             I NEW TABLE CREATION              */
/*************I********************I***********************************/

SET PATH *LIBL;

CREATE TABLE PLANMPSFOURTHCHAR ( 
	PLANIDFIRSTCHAR				VARCHAR(1) 
	) ;
	
RENAME PLANMPSFOURTHCHAR TO SYSTEM NAME PLNMSPFTHC;
-----------------------------------------------------------------------------------------	
	
INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( '6' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'I' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'O' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'F' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'S' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'W' ) ; 

INSERT INTO PLANMPSFOURTHCHAR ( PLANIDFIRSTCHAR ) VALUES ( 'E' ) ; 
