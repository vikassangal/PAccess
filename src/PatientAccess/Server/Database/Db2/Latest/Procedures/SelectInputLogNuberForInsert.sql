                                                                                
/**********************************************************************/        
/* AS400 Short Name: SELEC00014                                       */        
/* iSeries400        SELECTINPUTLOGNUMBERFORINSERT -SQL PROC          */        
/*                                                  FOR PATIENT ACCESS*/        
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
                                                                                
CREATE PROCEDURE SELECTINPUTLOGNUMBERFORINSERT ( )                              
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELINPLOG                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN ATOMIC                                                       
                                                                                
        UPDATE HPADAPHD SET APINLH = APINLH + 1 ;                               
                                                                                
        END P1  ;                                                               
