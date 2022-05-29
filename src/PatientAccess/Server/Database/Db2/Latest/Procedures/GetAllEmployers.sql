 /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETALLEMPLOYERS - SQL PROC FOR PX               */        
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
/* 11/18/2008 I  Sanjeev Kumar     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        

SET PATH *LIBL ;                                                                

CREATE PROCEDURE GETALLEMPLOYERS ( IN @P_FUUN VARCHAR(3) )                                               
                                                                                
          DYNAMIC RESULT SETS 1                                                 
          LANGUAGE SQL                                                          
          SPECIFIC GETALLEMPS                                                     
          NOT DETERMINISTIC                                                     
          READS SQL DATA           
                                                       
   P1 : BEGIN                                                                   
     DECLARE CURSOR1 CURSOR FOR
                                                      
          SELECT DISTINCT 
			TRIM(EMPLOYERS.EMNAME) AS EMPLOYERNAME,
			EMPLOYERS.EMCODE AS EMPLOYERCODE,
			TRIM(EMPLOYERS.EMNEID) AS NATIONALID,
			EMPLOYERS.EMUSER AS ADDEDBYUSERID
			
          FROM EMPLOYERSFORAPPROVAL EMPLOYERS                                                       
          
          WHERE EMPLOYERS.EMFUUN = @P_FUUN    
            AND EMPLOYERS.EMDLDT = 0
          
          ORDER BY EMPLOYERNAME;
           
          OPEN CURSOR1 ;                                                       
 END P1  ;                 