                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  GETACCOUNTACTIVITYHISTORY - SQL PROC FOR PAS         */        
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
/* This stored procedure returns all the occurrences of the           */
/* NWACT Activity code from the FUS-Activity table PADATA.AC0005P     */
/* for the given Account Number(P_ACCOUNTNUMBER) and Facility         */
/* Code(P_HSPCODE) to establish the Account Activity History.         */
/*                                                                    */
/*************I********************I***********************************/        
/*  Date      I  Programmer        I  Modification Description        */        
/*************I********************I***********************************/        
/* 06/14/2010 I  Deepa Raghavaraju I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE GETACCOUNTACTIVITYHISTORY(                                             
    IN P_HSPCODE CHAR(3) ,
    IN P_ACCOUNTNUMBER VARCHAR(9) ) 
	
    DYNAMIC RESULT SETS 1                                                   
    LANGUAGE SQL                                                            
    SPECIFIC GTACTHSTRY                                                       
    NOT DETERMINISTIC                                                       
    MODIFIES SQL DATA                                                       
    CALLED ON NULL INPUT                                                    
    SET OPTION DBGVIEW = *SOURCE                                            

P1 : BEGIN                                                              

    DECLARE ACCOUNTACTIVITY CURSOR FOR	

        SELECT 

            ACACTD	AS ACTIVITYDESCRIPTION

        FROM 

           AC0005P 

        WHERE 

            ACHSPC = P_HSPCODE
            AND
            ACPT#9 = P_ACCOUNTNUMBER
            AND
            ACACTC = 'NWACT';

    OPEN ACCOUNTACTIVITY;

END P1;
