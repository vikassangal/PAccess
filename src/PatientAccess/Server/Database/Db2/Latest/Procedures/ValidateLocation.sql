                                                                                
/**********************************************************************/        
/* AS400 Short Name: VALIDATELO                                       */        
/* iSeries400        VALIDATELOCATION -SQL PROC FOR PATIENT ACCESS    */        
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
                                                                                
CREATE PROCEDURE VALIDATELOCATION (                                             
        IN P_NEW_NURSINGSTATION VARCHAR(2) ,                                    
        IN P_NEW_ROOM VARCHAR(5) ,                                              
        IN P_NEW_BED VARCHAR(5) ,                                               
        IN P_FACILITYID INTEGER ,                                               
        OUT O_LOCATIONRESULT VARCHAR(100) )                                     
        LANGUAGE SQL                                                            
        SPECIFIC VALLOC                                                         
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        BEGIN                                                                   
                                                                                
        DECLARE NSCOUNT INTEGER DEFAULT 0 ;                                     
                                                                                
        DECLARE ROOMCOUNT       INTEGER DEFAULT 0 ;                             
                                                                                
        DECLARE BEDCOUNT        INTEGER DEFAULT 0 ;                             
                                                                                
        SELECT COUNT ( LRNS ) INTO NSCOUNT FROM HPADLRP                         
        WHERE LRHSP# = P_FACILITYID                                             
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) ;                                       
                                                                                
        IF NSCOUNT > 0 THEN                                                     
                                                                                
        SELECT COUNT ( LRROOM ) INTO ROOMCOUNT                                  
        FROM HPADLRP WHERE LRHSP# = P_FACILITYID                                
        AND LRROOM = P_NEW_ROOM
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) ;               
                                                                                
        IF ROOMCOUNT > 0 THEN                                                   
                                                                                
        SELECT COUNT ( LRBED ) INTO BEDCOUNT FROM HPADLRP                       
        WHERE LRHSP# = P_FACILITYID                                             
        AND trim(LRNS) = trim(P_NEW_NURSINGSTATION) 
        AND LRROOM = P_NEW_ROOM              
        AND LRBED = P_NEW_BED;                                                  
                                                                                
        IF BEDCOUNT = 0 THEN                                                    
                                                                                
        SET O_LOCATIONRESULT = 'Invalid Bed' ;                                  
                                                                                
        END IF ;                                                                
                                                                                
        ELSE                                                                    
                                                                                
        SET O_LOCATIONRESULT = 'Invalid Room' ;                                 
                                                                                
        END IF ;                                                                
                                                                                
        ELSE                                                                    
                                                                                
        SET O_LOCATIONRESULT = 'Invalid Nursingstation' ;                       
                                                                                
        END IF ;                                                                
                                                                                
        END  ;                                                                  
