                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  WORKLISTITEMIDFOR - SQL FUNCTION FOR PX              */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW FUNC                         */        
/*************I********************I***********************************/        
SET PATH *LIBL ;                                                                
                                                                                
CREATE FUNCTION WORKLISTITEMIDFOR (                                             
        P_ACCOUNTNUMBER INTEGER ,                                               
        P_FACILITYID INTEGER ,                                                  
        P_WORKLISTID INTEGER )                                                  
        RETURNS INTEGER                                                         
        LANGUAGE SQL                                                            
        SPECIFIC WRKLSTIFOR                                                     
        DETERMINISTIC                                                           
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        NO EXTERNAL ACTION                                                      
        SET OPTION DBGVIEW = *SOURCE                                            
        BEGIN ATOMIC                                                            
                                                                                
        DECLARE WLITEMID INTEGER DEFAULT 0 ;                                    
                                                                                
        SELECT WORKLISTITEMID INTO WLITEMID                                     
        FROM WORKLISTITEMS WLI                                                  
        WHERE ( WLI . WORKLISTID = P_WORKLISTID )                               
        AND ( WLI . ACCOUNTNUMBER = P_ACCOUNTNUMBER )                           
        AND ( WLI . FACILITYID = P_FACILITYID ) ;                               
                                                                                
                                                                                
        RETURN WLITEMID ;                                                       
        END  ;                                                                  
