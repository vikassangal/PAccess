                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  FindInvalidDates     - SQL PROC FOR PATIENT ACCESS   */        
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
/* 05/14/2007 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
 
 CREATE PROCEDURE FINDINVALIDDATES ( )                                            
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC FINDINVALIDDATES                                                         
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        set option dbgview = *source                                            
        P1 : BEGIN    
                                                             
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT LPHSP#, LPACCT, LPADT1, LPADT2, LPADT3, LPADT4
        FROM HPADLPP
        WHERE  LPLAT < 0
        OR LPADT1 < 0
        OR LPADT2 < 0
        OR LPADT3 < 0
        OR LPLDD < 0
		OR LPLDD < 0
        OR LPADT1 > 12
		OR ((LPADT1 in (1,3,5,7,8,10,12)) AND LPADT2 > 31)
		OR ((LPADT1 in (4,6,9,11)) AND LPADT2 > 30)
		OR ( LPADT1 = 2 
			  AND 
			  (
				(
					LPADT2 > 29
				)
				OR
				(
					(LPADT3 >= 50 AND MOD((LPADT3 + 1900),4) != 0 AND LPADT2 > 28)
					OR 
					(LPADT3 <  50 AND MOD((LPADT3 + 2000),4) != 0 AND LPADT2 > 28)
				) 
			 )
		   ); 
		  
		  DECLARE CURSOR2 CURSOR FOR 
		  SELECT PMHSPC, PMPT#9, PMADTD, PMDSDT
		  FROM PMP002P
		  WHERE PMADTD < 0
		  OR PMDSDT < 0;
			
			OPEN CURSOR1;
			OPEN CURSOR2;

			
			END P1;
 