/*************I********************************************************/
/*            I                                                       */
/* iSeries400 I  PLNCATGIES  - TABLE FOR PX                           */
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
/*************I********************I***********************************/
/* 2/20/2008  I  SOPHIE	           I New Table in Turnover            */
/*************I********************I***********************************/

SET PATH *LIBL;

CREATE TABLE PLANCATEGORIES (
	PLANCATEGORYID			FOR COLUMN PLNCTGRYID INTEGER NOT NULL ,
	PLANCATEGORYDESCRIPTION		FOR COLUMN PLNCTGRYDE VARCHAR(32),
	EXPECTEDASSOCIATEDNUMBERLABEL	FOR COLUMN NMBRLBL    VARCHAR(32),
	DISPLAYORDER			FOR COLUMN DSPLYRDR   INTEGER,
	CONSTRAINT PK_PLANCATEGORYID PRIMARY KEY( PLANCATEGORYID ) ) ;
	
RENAME PLANCATEGORIES TO SYSTEM NAME PLNCATGIES;        