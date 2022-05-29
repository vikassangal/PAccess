                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTSTATUS - SQL PROC FOR PX                       */        
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
/* 01/28/2006 I  Kevin Sedota      I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPLACEOFWORSHIPWITH ( 
	IN P_FACILITYID INTEGER,
	IN P_CODE VARCHAR(3))                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPLWORW                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
              DECLARE CURSOR1 CURSOR FOR                              
					SELECT 0 AS OID , 
					TRIM ( QTKEY ) AS CODE , 
					TRIM ( QTPARD ) AS DESCRIPTION, 
					QTHSP# AS FACILITYID 
					FROM HPADQTPZ 
					WHERE QTHSP# = P_FACILITYID  
					AND QTKEY = P_CODE;                                         
               OPEN CURSOR1 ;                                          
        END P1  ;                                               
 