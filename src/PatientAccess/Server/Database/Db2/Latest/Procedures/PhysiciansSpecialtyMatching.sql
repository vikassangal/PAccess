                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PHYSICIANSSPECIATYMATCHING - SQL PROC FOR PX         */        
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
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE PHYSICIANSSPECIALTYMATCHING (                                  
        IN P_FACILITYID INTEGER ,                                               
        IN P_PHYSICIANSPECIALITY VARCHAR(55) )                                  
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC PHYSPCMCH                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE CURSOR1 CURSOR FOR                                      
                SELECT DISTINCT                                                 
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
                WHERE TRIM ( S.QTDSPD ) =                                       
                        TRIM ( P_PHYSICIANSPECIALITY )                          
                AND MRHSP# = P_FACILITYID;                                   
                OPEN CURSOR1 ;                                                  
                END P1  ;                                                       
