/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  BENEFITCATEGORIES - TABLE FOR PATIENT ACCESS         */      
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
/* 02/04/2008 I  Jenney Alexandria      I NEW TABLE CREATION          */        
/*************I********************I***********************************/ 
----------------------------------------------------------------------------------------
-- define table

CREATE TABLE BENEFITCATEGORIES ( 
	BENEFITCATEGORYID				FOR COLUMN BNTCTGRYID INTEGER NOT NULL , 
	DESCRIPTION						FOR COLUMN BNTCTGRYDE VARCHAR(20) CCSID 37 DEFAULT NULL , 
	CONSTRAINT PK_BENEFITCATEGORYID PRIMARY KEY( BENEFITCATEGORYID ) ) ;  

RENAME BENEFITCATEGORIES TO SYSTEM NAME BNFTCTGRYS; 
	
-----------------------------------------------------------------------------------------	
-- populate data	
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
12, 'General'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
1, 'Inpatient'); 	
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
2, 'Outpatient'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
3, 'OB'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
4, 'Newborn'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
5, 'NICU'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
6, 'Psych IP'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
7, 'Psych OP'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
8, 'Chem Dep'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
9, 'SNF/Subacute'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
10, 'Rehab IP'); 
INSERT INTO BENEFITCATEGORIES ( BENEFITCATEGORYID, DESCRIPTION ) VALUES ( 
11, 'Rehab OP'); 

 
 