                                                                                 
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
                                                                                
CREATE PROCEDURE SELECTTXTIONHEADERINFO2 (                           

        IN P_INSURANCETXN INTEGER ,
        IN P_NONINSURANCETXN INTEGER , 
        IN P_OTHERTXN INTEGER , 
        IN P_HSP INTEGER,
		OUT O_RRNUM INTEGER,
		OUT O_LOGNUM INTEGER,
		OUT O_INSRRNUM INTEGER,
		OUT O_OTHERRRNUM INTEGER )   
        LANGUAGE SQL
        SPECIFIC SELTRHDR2 
        NOT DETERMINISTIC 
        MODIFIES SQL DATA 
        CALLED ON NULL INPUT 
        SET OPTION DBGVIEW =*SOURCE 
        
        P1 : BEGIN ATOMIC  

        DECLARE RR# INTEGER ; 
        DECLARE INSRR# INTEGER ; 
        DECLARE LOGNUMBER INTEGER ; 
        DECLARE OTHERRECNO INTEGER ; 

        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        APNXA1, -- APNXA1 = Next key for Log File 1 ( Patient RR # {seed} ) 
        APINLH, -- APINLH = Input log number (the lognum for this group of txns)   
        APNXA2, -- APNXA2 = Next key for Log File 2 ( Insurance RR # seed )        
        APNXA4  -- APNXA4 = Next key for Log File 4 ( Other RR # seed )
        FROM HPADAPHD                                                           
        FOR UPDATE ;                                                        

		SET O_RRNUM = null;
		SET O_LOGNUM = null;
		SET O_INSRRNUM = null;
		SET O_OTHERRRNUM = null;
                                 
        OPEN CURSOR1 ;                                                          
        FETCH CURSOR1 INTO RR# ,                                                
        LOGNUMBER ,                                                             
        INSRR# ,                                                                
        OTHERRECNO ; 

        UPDATE HPADAPHD SET 
      -- bump the patient rr # seed by the number of non-ins txns  
        APNXA1 = APNXA1 + P_NONINSURANCETXN ,        
      -- bump the log # by 1  
        APINLH = APINLH + 1 ,                       
      -- bump the insurance rr # seed by the number of ins txns
        APNXA2 = APNXA2 + P_INSURANCETXN ,           
      -- bump the other rr # seed by the number of other txns
        APNXA4 = APNXA4 + P_OTHERTXN           
        Where CURRENT OF CURSOR1 ;

        CLOSE CURSOR1;

        SELECT RR# ,LOGNUMBER ,  INSRR# , OTHERRECNO
	INTO O_RRNUM, O_LOGNUM, O_INSRRNUM, O_OTHERRRNUM
	FROM SYSIBM/SYSDUMMY1 ;

        END P1  ;                                                               
