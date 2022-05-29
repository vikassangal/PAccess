/*************I********************************************************/                                    
/*            I                                                       */                                    
/* iSeries400 I  SELECTCODEDDIAGSFORPAT    - SQL PROC FOR PX          */                                    
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
                                                                                                           
CREATE PROCEDURE SELECTCODEDDIAGSFORPAT (                                                                   
       IN P_HSP INTEGER,                                                                                    
       IN P_ACCTNUMBER INTEGER,                                                                             
       IN P_MRC INTEGER)                                                                                    
         DYNAMIC RESULT SETS 1                                                                             
        LANGUAGE SQL                                                                                       
        SPECIFIC SELCDFPA                                                                                  
        NOT DETERMINISTIC                                                                                  
        READS SQL DATA                                                                                     
        CALLED ON NULL INPUT                                                                               
        SET OPTION DBGVIEW = *SOURCE                                                                        
        P1 : BEGIN                                                                                         
                                                                                                            
               DECLARE CURSOR1 CURSOR FOR                                                                   
                                                                                                            
               SELECT RVCD01 as CD01, RVCD02 as CD02,                                                       
                       RVCD03 as CD03, RVCD04 as CD04,                                                      
                       RVCD05 as CD05, RVCD06 as CD06,                                                      
                       RVCD07 as CD07, RVCD08 as CD08,                                                      
                       RVRA01 as RA01, RVRA02 as RA02,                                                      
                       RVRA03 as RA03, RVRA04 as RA04,                                                      
                       RVRA05 as RA05                                                                       
               FROM HXMRRVP                                                                                 
               WHERE RVHSP# = P_HSP                                                                         
               AND RVACCT = P_ACCTNUMBER                                                                    
               AND RVMRC# = P_MRC;                                                                          
                                                                                                            
               OPEN CURSOR1;                                                                                
                                                                                                            
        END P1;                                                                                             
