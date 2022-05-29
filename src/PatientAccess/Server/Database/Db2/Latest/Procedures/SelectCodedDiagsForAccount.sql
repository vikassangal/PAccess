/*************I********************************************************/                                    
/*            I                                                       */                                    
/* iSeries400 I  SELECTCODEDDIAGSFORACCT   - SQL PROC FOR PX          */                                    
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
/* 11/03/2006 I  Kevin Sedota      I NEW STORED PROCEDURE             */                                    
/*************I********************I***********************************/                                    
                                                                                                            
SET PATH *LIBL ;                                                                                           
                                                                                                           
CREATE PROCEDURE SELECTCODEDDIAGSFORACCT (                                                                  
       IN P_HSP INTEGER,                                                                                    
       IN P_ACCTNUMBER INTEGER,                                                                             
       IN P_MRC INTEGER)                                                                                    
         DYNAMIC RESULT SETS 1                                                                             
        LANGUAGE SQL                                                                                       
        SPECIFIC SELCDFAC                                                                                  
        NOT DETERMINISTIC                                                                                  
        READS SQL DATA                                                                                     
        CALLED ON NULL INPUT                                                                               
        SET OPTION DBGVIEW = *SOURCE                                                                        
        P1 : BEGIN                                                                                         
                                                                                                            
               DECLARE CURSOR1 CURSOR FOR                                                                   
                                                                                                            
               SELECT LPCD01 as CD01, LPCD02 as CD02,                                                       
                      LPCD03 as CD03, LPCD04 as CD04,                                                       
                      LPCD05 as CD05, LPCD06 as CD06,                                                       
                      LPCD07 as CD07, LPCD08 as CD08,                                                       
                      LPRA01 as RA01, LPRA02 as RA02,                                                       
                      LPRA03 as RA03, LPRA04 as RA04,                                                       
                      LPRA05 as RA05                                                                        
               FROM HPADLPP                                                                                 
               WHERE LPHSP# = P_HSP                                                                         
               AND LPACCT = P_ACCTNUMBER                                                                    
               AND LPMRC# = P_MRC;                                                                          
                                                                                                            
               OPEN CURSOR1;                                                                                
                                                                                                            
        END P1;                                                                                             
                                                                                                            
                                                                                                            
