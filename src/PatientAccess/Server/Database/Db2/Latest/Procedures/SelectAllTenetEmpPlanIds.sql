﻿                                                                                  
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SLALTNID.sql - SQL PROC FOR PX        */        
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
/* 01/15/2008 I  JITHIN A     I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTALLTENETEMPPLANIDS()                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SLALTNID                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT   
                                                         
        SET OPTION DBGVIEW = *SOURCE
                                                    
        P1 : BEGIN              
                                                        
			DECLARE CURSOR1 CURSOR FOR                                      
        
			 SELECT 		DISTINCT
			0			AS OID,
			TRIM(PLANID) AS CODE,
			'' AS DESCRIPTION
			FROM TENETPLANID;
			
			
		
	                                
			OPEN CURSOR1 ; 
			                                                 
        END P1  ;                                                       
