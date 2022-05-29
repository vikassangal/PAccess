                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETFINANCIALDATAFORACCOUNT - SQL PROC FOR PX         */        
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
                                                                                
CREATE PROCEDURE GETFINANCIALDATAFORACCOUNT (                                   
        IN P_ACCOUNTNUMBER INTEGER ,                                            
        IN P_HSPCODE CHAR(3) )                                                  
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC GETFINDATA                                                     
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT                                                          
                F.PMTPD9 AS TOTALPAID ,                                         
                F.PMDTBL AS BILLDROPPED ,                                       
                F.PMLSCD AS LASTCHARGEDATE                                      
                FROM PM0001P F                                                  
                WHERE F.PMHSPC = P_HSPCODE AND                                  
                F.PMPT#9 = P_ACCOUNTNUMBER ;                                    
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
