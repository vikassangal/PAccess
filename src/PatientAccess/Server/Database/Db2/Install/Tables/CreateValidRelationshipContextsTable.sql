/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  VALIDRELATIONSHIPCONTEXTS - TABLE FOR PATIENT ACCESS         */      
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
/* 02/04/2008 I  Jithin		     I NEW TABLE CREATION          */        
/*************I********************I***********************************/ 
----------------------------------------------------------------------------------------
-- define table
  
CREATE TABLE VALIDRELATIONSHIPCONTEXTS ( 
	TYPEOFROLEID FOR COLUMN TYPEROLEID INTEGER NOT NULL , 
	RELATIONSHIPCODE FOR COLUMN RELSHIPCDE VARCHAR(10) NOT NULL , 
	CONSTRAINT PK_VALIDRELATIONSHIPCONTEXTS PRIMARY KEY( RELATIONSHIPCODE , TYPEOFROLEID ) )   
	; 
	
RENAME VALIDRELATIONSHIPCONTEXTS TO SYSTEM NAME VLDRLSPCXT;      
