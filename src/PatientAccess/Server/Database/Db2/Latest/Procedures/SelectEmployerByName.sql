/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTEMPLOYERBYNAME - SQL PROC FOR PX               */        
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
/* 08/02/2006 I                    I NEW STORED PROCEDURE             */        
/* 11/13/2007 I Jithin A           I Added a check to filter expired 
									 employees			              */ 
/* 01/31/2008 I Sophie Zhang       I refactor for Goo project         */ 									   
/*************I********************I***********************************/        

SET PATH *LIBL ;                                                                

CREATE PROCEDURE SELECTEMPLOYERBYNAME (                                         
          IN @P_FUUN VARCHAR(3),                                                 
          IN @P_EMNAME VARCHAR(25) )                                               
                                                                                
          DYNAMIC RESULT SETS 1                                                 
          LANGUAGE SQL                                                          
          SPECIFIC SELEMPNM                                                     
          NOT DETERMINISTIC                                                     
          READS SQL DATA                                                        
   P1 : BEGIN                                                                   
     DECLARE CURSOR1 CURSOR FOR
                                                      
          SELECT DISTINCT 
			TRIM(EMPLOYERS.EMNAME) AS EMPLOYERNAME,
			EMPLOYERS.EMCODE AS EMPLOYERCODE,
			TRIM(EMPLOYERS.EMNEID) AS NATIONALID
          FROM NCEM10P EMPLOYERS                                                       
          WHERE EMPLOYERS.EMFUUN = @P_FUUN    
             AND EMPLOYERS.EMDLDT = 0                                              
             AND UPPER(TRIM(EMPLOYERS.EMNAME)) LIKE UPPER(TRIM(@P_EMNAME))                               
          ORDER BY EMPLOYERNAME;
           
          OPEN CURSOR1 ;                                                       
 END P1  ;                 