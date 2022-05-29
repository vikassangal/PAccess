                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTIPASBYNAME - SQL PROC FOR PX                    */        
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
/* 01/18/2008 I  Sophie Zhang      I  NEW STORED PROCEDURE            */        
/* 03/18/2008 I  Kevin Sedota      I  start using search name         */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTIPASBYNAME(                                                   
        IN @P_FACILITYID INTEGER,
        IN @P_IPANAME VARCHAR(15))                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELIPANM                                                        
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT 
					0 AS IPAID, 
					TRIM(IPAS.NHIPA) AS IPACODE, 
					TRIM(IPAS.NHIPAN) AS IPANAME, 
					0 AS CLINICID, 
					TRIM(IPAS.NHIPAC) AS CLINICCODE, 
					TRIM(IPAS.NHIPCN) AS CLINICNAME
				FROM NH0301P IPAS
				JOIN NOHLHSPP FACILITIES ON FACILITIES.HHHSPC = IPAS.NHHSPC  
				WHERE FACILITIES.HHHSPN = @P_FACILITYID
				AND UPPER(TRIM(IPAS.NHIPASN)) LIKE UPPER(TRIM(@P_IPANAME)) || '%' 
				--AND UPPER(TRIM(IPAS.NHIPAN)) LIKE UPPER(TRIM(@P_IPANAME)) || '%' 
				ORDER BY IPACODE, CLINICCODE;                                         
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
 