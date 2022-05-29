                                                                                 
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTALLSSNSTATUSES - SQL PROC FOR PX               */        
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
/* 01/28/2008 I  Sanjeev Kumar     I  NEW STORED PROCEDURE            */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPATIENTSMATCHINGSSN ( 
	        P_SSN IN VARCHAR( 11 ),
			P_HOSPITAL_CODE IN INT 
        )                                           

        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPAMASSN
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        
        P1 : BEGIN                                                              
        
                DECLARE CURSOR1 CURSOR FOR                                      
                
				SELECT	MDMRC#	AS MRN,
						TRIM ( MDPLNM )	AS LASTNAME,
						TRIM ( MDPFNM )	AS FIRSTNAME,
						TRIM ( MDPMI )	AS MIDDLEINITIAL,						
						MDDOB8	AS DOB,
						TRIM ( MDSEX )	AS SEXID
				FROM HPADMDP
				WHERE	MDHSP#	= P_HOSPITAL_CODE AND
						MDSSN	= P_SSN AND
						P_SSN NOT IN ('000000000','000000001','111111111','555555555','777777777','999999999')
				ORDER BY LASTNAME, FIRSTNAME ;
                
                OPEN CURSOR1 ;                                                  
                
        END P1  ;
