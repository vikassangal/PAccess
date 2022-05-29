                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PHYSICIANSMATCHING - SQL PROC FOR PX                 */        
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
/* 04/28/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/* 03/18/2006 I  Kevin Sedota      I start using search names         */        
/*************I********************I***********************************/        
                                                                                
/* SET PATH *LIBL ;  */                                                         
                                                                                
CREATE PROCEDURE PHYSICIANSMATCHING (                                           
        IN P_FACILITYID INTEGER ,                                               
        IN P_LASTNAME VARCHAR(55) ,                                             
        IN P_FIRSTNAME VARCHAR(55) ,                                            
        IN P_PHYSICIANNUMBER INTEGER )                                          
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC PHYMCH                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT                                                                  
        MRDNM as PhysicianName,
                MRDR# as PhysicianNumber,
                MRLNM as PhysicianLastname,
                MRFNM as PhysicianFirstName,
                MRMI  as PhysicianMiddleInitial,
                MRHSP# as FacilityID, 
                MRUPI# as UPIN,
                MRINA as ACTIVEFLAG,
                MRMDAP as ADMITPRIVILEGE,
                MRMPDT as ADMITPRIVSUSPENDDATE,
                MRSPCD  as PHYSICIANSPECIALITYCODE,
                S.QTDSPD AS PHYSICIANSPECIALITYDESC,
                MRSLIC as STATELICENSENUMBER,
                E.EXDATE as EXCLUDEDATE,
                MRSTAT as STATUSCODE,
                ST.QTDSTD as STATUSDESCRIPTION                                      
        FROM HPADMRP   
        left outer JOIN HPADQTDS S on S.QTKEY = MRSPCD and S.QTHSP# = 999
        left outer JOIN NDEXMDP E on EXHSP# = MRHSP# and EXDR# = MRDR#
        left outer JOIN HPADQTDC ST on ST.QTKEY = MRSTAT and ST.QTHSP# = 999                                                         
        WHERE MRHSP# = P_FACILITYID                                             
        AND ( ( MRDR# IS NULL                                                   
        OR MRDR# = P_PHYSICIANNUMBER )                                          
        OR ( ( ( P_LASTNAME IS NOT NULL )                                       
        AND ( P_FIRSTNAME IS NULL )                                             
        --AND ( UPPER ( MRLNM )                                                   
        AND ( UPPER ( MRLSN )                                                   
        LIKE CONCAT ( UPPER ( P_LASTNAME ) ,                                    
                '%' ) ) )                                                       
        OR ( ( P_FIRSTNAME IS NOT NULL )                                        
        AND ( P_LASTNAME IS NOT NULL )                                          
        --AND ( ( UPPER ( MRFNM )                                                 
        AND ( ( UPPER ( MRFSN )                                                 
        LIKE CONCAT ( UPPER ( P_FIRSTNAME ) , '%' ) )                           
        --AND ( UPPER ( MRLNM )                                                   
        AND ( UPPER ( MRLSN )                                                   
        LIKE CONCAT ( UPPER ( P_LASTNAME ) , '%' ) ) ) ) ) )                    
        ORDER BY                                                                
        MRLNM ,                                                                 
        MRFNM ,                                                                 
        MRMI ;                                                                  
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
