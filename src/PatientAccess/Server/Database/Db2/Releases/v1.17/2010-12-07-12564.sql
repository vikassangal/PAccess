/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  HSVBENEFITCATEGORYXREF - TABLE FOR PATIENT ACCESS    */      
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
/* 12/09/2010 I Deepa Raju         I Task 12564 - Included new HSV 57 */       
/*************I********************I***********************************/ 
----------------------------------------------------------------------------------------
-- Add new Hospital Service Code '57' to XREF table
	

DELETE FROM HSVBENEFITCATEGORYXREF WHERE SERVICECODE = '57' AND BENEFITCATEGORYID = 2;

INSERT INTO HSVBENEFITCATEGORYXREF (SERVICECODE, BENEFITCATEGORYID) VALUES ('57', 2);
