                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLPATIENTTYPES - SQL PROC FOR PX              */        
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
/* 02/05/2008 I  GAUTHREAUX        I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTALLPATIENTTYPES ( )                                 
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELALLPTYP                                                       
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT
                                                            
        SET OPTION DBGVIEW = *SOURCE   
                                                
        P1 : BEGIN                                                              
                
            DECLARE CURSOR1 CURSOR FOR 
                                                
            SELECT	0			AS OID,
					TRIM(QTKEY)	AS CODE,                                   
					TRIM(QTTAD)	AS DESCRIPTION
					
            FROM	HPADQTTA
            
            WHERE	QTHSP# = 999 
            
            UNION 
            
			SELECT	0			AS OID, 
					''			AS CODE, 
					''			AS DESCRIPTION
					
			FROM	SYSIBM/SYSDUMMY1
					
			ORDER BY CODE;                                            
            
        OPEN CURSOR1 ;                                                  
 
        END P1  ;                                                       
 