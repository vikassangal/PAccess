                                                                                  
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTPLANFINCLASSESWITH - SQL PROC FOR PX           */        
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
/* 01/24/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/* 05/09/2008 I  Sanjeev Kumar	   I  MADE SEARCH MORE INCLUSIVE      */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPLANFINCLASSESWITH( 
		IN @P_SUFFIX VARCHAR( 2 ) )                                               
        
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPLNFINW                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        
        P1 : BEGIN

				DECLARE PLAN_SUFFIX_GENERIC_CONST VARCHAR( 2 );				

				DECLARE CURSOR1 CURSOR FOR 
				                                 				
					SELECT PLANFINXREF.FCALLOWFC AS FINANCIALCLASSCODE
						FROM NHADFCP PLANFINXREF
						WHERE TRIM( PLANFINXREF.FCPLI2 ) = @P_SUFFIX
					UNION
					SELECT PLANFINXREF.FCALLOWFC AS FINANCIALCLASSCODE
						FROM NHADFCP PLANFINXREF
						WHERE TRIM( PLANFINXREF.FCPLI2 ) = SUBSTRING( @P_SUFFIX, 1, 1 ) || '*'
						ORDER BY FINANCIALCLASSCODE;           
                                                                                     
                SET PLAN_SUFFIX_GENERIC_CONST = '00';
				IF ( @P_SUFFIX = PLAN_SUFFIX_GENERIC_CONST ) THEN
					SET @P_SUFFIX = '';
				END IF ;

                OPEN CURSOR1 ; 
                                                                 
                END P1  ;                                                       
 