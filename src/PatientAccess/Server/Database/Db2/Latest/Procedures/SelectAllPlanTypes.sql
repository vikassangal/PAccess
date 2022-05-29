                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLPLANTYPES - SQL PROC FOR PX                 */        
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
/* 01/17/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTALLPLANTYPES()                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPLNTYP                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE CURSOR1 CURSOR FOR
                SELECT OID, CODE, DESCRIPTION
                FROM                                      
                (SELECT 
					0 AS OID, 
					TRIM(PLANTYPES.PTPLTY) AS CODE, 
					TRIM(PLANTYPES.PTPTDS) AS DESCRIPTION 
					FROM NKPT10P PLANTYPES
				UNION ALL
				SELECT 
					0 AS OID, 
					'' AS CODE,
					'' AS DESCRIPTION
					FROM SYSIBM/SYSDUMMY1)
				AS V(OID, CODE, DESCRIPTION)
				ORDER BY CODE;                                  
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
 