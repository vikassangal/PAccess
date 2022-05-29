                                                                                  
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTTXTIONHEADERINFO2 - SQL PROC FOR PX            */        
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
/* 02/15/2008 I  Kevin Sedota      I Create   STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
           

CREATE PROCEDURE DELETEWRKLSTPROCESSINGRECFOR (                           
        	IN P_HSP INTEGER,
	IN P_ACCOUNTNUMBER INTEGER )   
        LANGUAGE SQL
        SPECIFIC DELWKLSTRC 
        NOT DETERMINISTIC 
        MODIFIES SQL DATA 
        CALLED ON NULL INPUT 
        SET OPTION DBGVIEW =*SOURCE 

        P1 : BEGIN ATOMIC  
        DELETE FROM NHUPDPASP 
        WHERE  PASHSP# = P_HSP
       AND PASACCT = P_ACCOUNTNUMBER;

        END P1  ;                                                               
