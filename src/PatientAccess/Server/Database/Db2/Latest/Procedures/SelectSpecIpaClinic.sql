                                                                                
/**********************************************************************/        
/* AS400 Short Name: SELEC00006                                       */        
/* iSeries400        SELECTSPECIPACLINIC -SQL PROC FOR PATIENT ACCESS */        
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
                                                                                
CREATE PROCEDURE SELECTSPECIPACLINIC (                                          
        IN @NHHSPC VARCHAR(3) ,                                                 
        IN @IPACODE VARCHAR(5) ,                                                
        IN @CLINICCODE VARCHAR(2) )                                             
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELSPECIP2                                                     
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT NHIPA , NHIPAC , NHIPAN , NHIPCN                                 
        FROM NH0301P                                                            
        WHERE NHIPA = @IPACODE                                                  
        AND NHIPAC = @CLINICCODE                                                
        AND NHHSPC = @NHHSPC ;                                                  
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
