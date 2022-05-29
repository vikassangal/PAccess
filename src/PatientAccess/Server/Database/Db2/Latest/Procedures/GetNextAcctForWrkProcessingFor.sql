                                                                                 
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

CREATE PROCEDURE GETNEXTACCTFORWRKPROCESSINGFOR (                           
         IN P_HSP INTEGER,
		OUT O_ACCOUNTNUMBER INTEGER,
		OUT O_MEDICALRECORDNUMBER INTEGER,
		OUT O_ACTIVITY CHAR )   
        LANGUAGE SQL
        SPECIFIC GETNXWKAC 
        NOT DETERMINISTIC 
        MODIFIES SQL DATA 
        CALLED ON NULL INPUT 
        SET OPTION DBGVIEW =*SOURCE 

        P1 : BEGIN ATOMIC
		DECLARE ACCOUNTNUMBER INTEGER ;
		DECLARE ACTIVITY CHAR ;

		DECLARE CURSOR1 CURSOR FOR
		SELECT PASACCT , PASSTATUS
		FROM NHUPDPASP
		WHERE PASHSP# = P_HSP
		AND PASPROCESS = ' '
		AND PASACCT <> 0
		AND PASDATESTM =
			(
			SELECT MIN ( PASDATESTM ) FROM NHUPDPASP 
			WHERE PASHSP# = P_HSP
			AND PASPROCESS = ' '
			AND PASACCT <> 0
			)
		FOR UPDATE ;
			SET O_ACTIVITY = NULL ;
			SET O_ACCOUNTNUMBER = NULL ;
			SET O_MEDICALRECORDNUMBER = NULL ;

		OPEN CURSOR1 ;

		FETCH CURSOR1 INTO ACCOUNTNUMBER , ACTIVITY ;

		IF ACCOUNTNUMBER <> 0 THEN
		UPDATE NHUPDPASP SET
		PASPROCESS = 'P'
		WHERE CURRENT OF CURSOR1 ;
		CLOSE CURSOR1 ;

		SELECT ACCOUNTNUMBER , ACTIVITY
			INTO O_ACCOUNTNUMBER , O_ACTIVITY
			FROM SYSIBM / SYSDUMMY1 ;

		SELECT LPMRC# INTO O_MEDICALRECORDNUMBER
		FROM HPADLPP
		WHERE LPHSP# = P_HSP
		AND LPACCT = ACCOUNTNUMBER ;

		END IF ;
		END P1                                                              
