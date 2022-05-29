                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELPROPLOC - SQL PROC FOR PX                       */        
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
/* 04/21/2008 I  Kevin Sedota     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTPREVOPLOCATION ( 
	IN P_HSP INTEGER,
	IN P_ACCOUNTNUMBER INTEGER)                           
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELPROPLOC                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
        
        DECLARE CURSOR1 CURSOR FOR                              
        SELECT LPOBNS	as OBSERVATIONNS,
	    LPOBROOM		as OBSERVATIONROOM,
	    LPOBBED		as OBSERVATIONBED
        FROM HPADLPP
	    WHERE LPHSP# = P_HSP
	    AND LPACCT = P_ACCOUNTNUMBER ; 
                                         
        OPEN CURSOR1 ;                                          
        END P1  ;                                               
