                                                                                
/**********************************************************************/        
/* AS400 Short Name: TXNCOUNTFO                                       */        
/* iSeries400        TXNCOUNTFOR -SQL PROC FOR PATIENT ACCESS         */        
/*                                                                    */        
/*    ************************************************************    */        
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */        
/*    *                                                          *    */        
/*    * This unpublished material is proprietary to Perot Sys.   *    */        
/*    * The methods and techniques described herein are          *    */        
/*    * considered trade secrets and/or confidential.            *    */        
/*    * Reproduction or distribution, in whole or in part, is    *    */        
/*    * forbidden except by express written permission of        *    */        
/*    * Perot Systems, Inc.                                      *    */        
/*    ************************************************************    */        
/*                                                                    */        
/*                                                                    */        
/***********I**********I***********I***********************************/        
/*  Date    I Request #I  Pgmr     I  Modification Description        */        
/***********I**********I***********I***********************************/        
/* 05/17/06 I 4043030  I  D Evans  I NEW PROC                         */        
/***********I**********I***********I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE TXNCOUNTFOR (                                                  
        IN P_HSP INTEGER ,                                                      
        IN P_ACCT INTEGER ,                                                     
        IN P_TXN VARCHAR(2) )                                                   
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC TXNCNTFOR                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE RECCNT INTEGER ;                                                
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT RECCNT AS RECCNT                                                 
        FROM SYSIBM / SYSDUMMY1 ;                                               
                                                                                
        SET RECCNT = 0 ;                                                        
                                                                                
        IF ( P_TXN = 'IO' OR P_TXN = 'OT' ) THEN                                
        SELECT COUNT ( * ) INTO RECCNT                                          
        FROM HPADAPOT                                                           
        WHERE APHSP# = P_HSP                                                    
        AND APACCT = P_ACCT                                                     
        AND ( APIDID = 'IO' OR APIDID = 'OT' ) ;                                
        ELSEIF ( P_TXN = 'CD' ) THEN                                            
        SELECT COUNT ( * ) INTO RECCNT                                          
        FROM HPADAPCP                                                           
        WHERE APHSP# = P_HSP                                                    
        AND APACCT = P_ACCT                                                     
        AND APIDID = 'CD' ;                                                     
        END IF ;                                                                
                                                                                
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
