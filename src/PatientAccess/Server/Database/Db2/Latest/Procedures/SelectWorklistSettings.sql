                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTWORKLISTSETTINGS - SQL PROC FOR PX             */        
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
/* 04/28/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTWORKLISTSETTINGS (                                       
        IN P_USERID INTEGER ,                                                   
        IN P_WORKLISTID INTEGER )                                               
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELWRKSET                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT                                                          
                STARTLETTERS ,                                                  
                ENDLETTERS ,                                                    
                WORKLISTSELECTIONRANGEID ,                                      
                WORKLISTID ,                                                    
                USERID ,                                                        
                STARTDATE ,                                                     
                ENDDATE ,                                                       
                SORTEDCOLUMN ,                                                  
                SORTEDCOLUMNDIRECTION                                           
                FROM WORKLISTSETTINGS WS                                        
                WHERE WS.USERID = P_USERID                                      
                AND WS.WORKLISTID = P_WORKLISTID ;                              
                                                                                
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
