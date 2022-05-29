                                                                                   
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTISVALIDHSVPLANMAPPING - SQL PROC FOR PX        */        
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
/* 01/24/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE			  */
/* 02/14/2008 I  Sophie Zhang      I  REPLACE CURSOR WITH OUT PARAM   */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTISVALIDHSVPLANMAPPING( 
		IN @P_SERVICECD VARCHAR(10),
		IN @P_PLANCD VARCHAR(5),
		OUT @O_SERVICECOUNT INTEGER)                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC VLDHSVPLN                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN
				DECLARE P_PLAN_CNT INTEGER DEFAULT 0;				
				DECLARE P_SERVICE_CNT INTEGER DEFAULT 0;
				DECLARE P_MAPPING_CNT INTEGER DEFAULT 0;
				
				SELECT COUNT(*)
                INTO P_PLAN_CNT
                FROM MEDICAREPLANHSVXREF
                WHERE TRIM(PLANID) = TRIM(@P_PLANCD);
                
                SELECT COUNT(*)
                INTO P_SERVICE_CNT
                FROM MEDICAREPLANHSVXREF
                WHERE TRIM(SERVICECODE) = TRIM(@P_SERVICECD); 
                
                IF (P_PLAN_CNT > 0 OR P_SERVICE_CNT > 0) THEN
                
					SELECT COUNT(*)
					INTO @O_SERVICECOUNT
					FROM MEDICAREPLANHSVXREF
					WHERE TRIM(PLANID) = TRIM(@P_PLANCD)
						AND TRIM(SERVICECODE) = TRIM(@P_SERVICECD);
					
				ELSE
					SET @O_SERVICECOUNT = 1;
				END IF;
				                                                                     
                END P1  ;                                                       
 