                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SAVEWORKLISTSETTINGS - SQL PROC FOR PX              */         
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
                                                                                
CREATE PROCEDURE SAVEWORKLISTSETTINGS (                                         
        IN P_USERID INTEGER ,                                                   
        IN P_STARTINGLETTERS VARCHAR(3) ,                                       
        IN P_ENDINGLETTERS VARCHAR(25) ,                                        
        IN P_STARTDATE DATE ,                                                   
        IN P_ENDDATE DATE ,                                                     
        IN P_WORKLISTID INTEGER ,                                               
        IN P_RANGETYPEID INTEGER ,                                              
        IN P_SORTEDCOLUMN INTEGER ,                                             
        IN P_SORTEDCOLUMNDIRECTION INTEGER )                                    
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SAVWRKSET                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                DELETE                                                          
                FROM WORKLISTSETTINGS WS                                        
                WHERE WS.USERID = P_USERID                                      
                AND WS.WORKLISTID = P_WORKLISTID ;                              
                                                                                
                INSERT INTO WORKLISTSETTINGS                                    
                (                                                               
                STARTLETTERS ,                                                  
                ENDLETTERS ,                                                    
                WORKLISTSELECTIONRANGEID ,                                      
                WORKLISTID ,                                                    
                USERID ,                                                        
                STARTDATE ,                                                     
                ENDDATE ,                                                       
                SORTEDCOLUMN ,                                                  
                SORTEDCOLUMNDIRECTION                                           
                )                                                               
                VALUES (                                                        
                P_STARTINGLETTERS ,                                             
                P_ENDINGLETTERS ,                                               
                P_RANGETYPEID ,                                                 
                P_WORKLISTID ,                                                  
                P_USERID ,                                                      
                P_STARTDATE ,                                                   
                P_ENDDATE ,                                                     
                P_SORTEDCOLUMN ,                                                
                P_SORTEDCOLUMNDIRECTION ) ;                                     
                END P1  ;                                                       
