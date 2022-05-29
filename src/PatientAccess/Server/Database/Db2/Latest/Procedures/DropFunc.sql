                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  DROP_FUNC - SQL PROC FOR PX                          */        
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
/* 04/27/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE DROP_FUNC (                                                    
        IN P_SCHNAME VARCHAR(32) ,                                              
        IN P_FUNCNAME VARCHAR(32) )                                             
        LANGUAGE SQL                                                            
        SPECIFIC DROPFUNC                                                       
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        CHK1 : BEGIN ATOMIC                                                     
        DECLARE STMT CHAR ( 3000 ) ;                                            
        DECLARE CNT BIGINT ;                                                    
        SET CNT = 0 ;                                                           
        SELECT COUNT ( * ) INTO CNT FROM QSYS2/SYSFUNCS                         
        WHERE ROUTINE_NAME = P_FUNCNAME                                         
        AND ROUTINE_SCHEMA = P_SCHNAME ;                                        
        IF CNT > 0 THEN                                                         
        SET STMT =                                                              
        'drop function ' || P_SCHNAME || '.' || P_FUNCNAME ;                    
        PREPARE S1 FROM STMT ;                                                  
        EXECUTE S1 ;                                                            
        END IF ;                                                                
        END CHK1  ;                                                             
