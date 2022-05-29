 /************I********************************************************/        
/*            I                   */        
/* iSeries400 I  SELECTALLCOUNTIESFOR -                               */
/*            I                  SQL PROC FOR PATIENT ACCESS          */      
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
/* 01/18/2008 I  Gauthreaux        I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/ 
/* 09/24/2012 I  Deepak Nair       I Added a new field - STATECODE    */        
/*************I********************I***********************************/ 
  
SET PATH *LIBL ; 
  
CREATE PROCEDURE SELECTALLCOUNTIESFOR ( 
    IN P_FACILITYID INTEGER ) 
    DYNAMIC RESULT SETS 1 
    LANGUAGE SQL 
    SPECIFIC SELALLCNTF 
    NOT DETERMINISTIC 
    READS SQL DATA 
    CALLED ON NULL INPUT 
    SET OPTION  ALWBLK = *ALLREAD , 
    ALWCPYDTA = *OPTIMIZE , 
    COMMIT = *NONE , 
    DBGVIEW = *SOURCE , 
    DECRESULT = (31, 31, 00) , 
    DFTRDBCOL = *NONE , 
    DYNDFTCOL = *NO , 
    DYNUSRPRF = *USER , 
    SRTSEQ = *HEX   
    BEGIN 
  
        DECLARE RESULT_CURSOR CURSOR FOR 
        
            SELECT 
                0            AS OID,
                ''           AS CODE,
                ''           AS DESCRIPTION,
				''			 AS STATECODE , 
                P_FACILITYID AS FACILITYID
            FROM 
                SYSIBM / SYSDUMMY1

            UNION ALL
                    
            SELECT 
                0               AS OID,
                TRIM ( QTKEY )  AS CODE,
                TRIM ( QTCNTN ) AS DESCRIPTION,
				TRIM ( QTSTATE ) AS STATECODE,
                QTHSP#          AS FACILITYID
            FROM 
                HPADQTCO
            WHERE 
                QTHSP# = P_FACILITYID
            ORDER BY 
                CODE;
                        
        OPEN RESULT_CURSOR;
    
    END;
