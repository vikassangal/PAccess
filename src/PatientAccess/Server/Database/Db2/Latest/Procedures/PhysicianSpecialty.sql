                                                                                
/**********************************************************************/        
/* AS400 Short Name: PHYSI00003                                       */        
/* iSeries400        PHYSICIANSPECIALTY -SQL PROC FOR PATIENT ACCESS  */        
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
                                                                                
CREATE PROCEDURE PHYSICIANSPECIALTY (                                           
        IN P_FACILITYID INTEGER )                                               
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC PHYSPC                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        A . QTDSPD AS PHYSICIANSPECIALITYDESC                                   
        , A . QTKEY AS PHYSICIANSPECIALITYCODE                                  
        FROM HPADQTDS A                                                         
        WHERE A . QTHSP# = P_FACILITYID ;                                       
                                                                                
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
