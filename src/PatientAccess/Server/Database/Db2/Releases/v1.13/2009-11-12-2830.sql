                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETACCOUNTCREATEDDATE - SQL PROC FOR PX            */        
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
/* 11/10/2009 I Smitha			   I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                        
CREATE PROCEDURE GETACCOUNTCREATEDDATE (
    IN P_HSPCODE CHAR(3) ,
    IN P_ACCOUNTNUMBER INTEGER  
    )                                               

        DYNAMIC RESULT SETS 1  
        LANGUAGE SQL                
        SPECIFIC GETACTCRDT       
        NOT DETERMINISTIC                
        MODIFIES SQL DATA              
        CALLED ON NULL INPUT                
        SET OPTION DBGVIEW = *SOURCE              
        P1 : BEGIN                                                              
             DECLARE CURSOR1 CURSOR FOR           
				SELECT 
					FUS.ACENTD AS ACCTCREATEDDATE 
				FROM 
					AC0005P FUS -- PADATA Library 
				WHERE 
					FUS.ACHSPC = P_HSPCODE
				AND  FUS.ACPT#9 = P_ACCOUNTNUMBER
				AND  FUS.ACACTC = 'NWACT' 
				ORDER BY FUS.ACENTD , FUS.ACENTT ASC 
				FETCH FIRST ROW ONLY;                                                                             
           OPEN CURSOR1 ;        
       END P1  ;                                               
