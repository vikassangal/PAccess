                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PHYSICIANSUMMARYFOR - SQL PROC FOR PX                */        
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
                                                                                
CREATE PROCEDURE PHYSICIANSUMMARYFOR (                                          
        IN P_FACILITYID INTEGER ,                                               
        IN P_PHYSICIANNUMBER INTEGER ,                                          
        OUT O_TOTAL_PATIENTS INTEGER ,                                          
        OUT O_TOTAL_ATTENDING INTEGER ,                                         
        OUT O_TOTAL_ADMITTING INTEGER ,                                         
        OUT O_TOTAL_REFERING INTEGER ,                                          
        OUT O_TOTAL_OPERATING INTEGER ,                                         
        OUT O_TOTAL_CONSULTING INTEGER )                                        
        DYNAMIC RESULT SETS 2                                                   
        LANGUAGE SQL                                                            
        SPECIFIC PHYSUMFOR                                                      
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                DECLARE REFDRID VARCHAR ( 5 ) ;                                 
                SET REFDRID = '' ;                                              
                SELECT LPAD                                                     
                ( VARCHAR ( P_PHYSICIANNUMBER ) , 5 , '0' )                     
                INTO REFDRID FROM SYSIBM/SYSDUMMY1 ;                            
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_PATIENTS                                           
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND ( ADMITTINGDRID = P_PHYSICIANNUMBER OR                      
                ATTENDINGDRID = P_PHYSICIANNUMBER OR                            
                TRIM ( REFERINGDRID ) = REFDRID OR                              
                OPERATINGDRID = P_PHYSICIANNUMBER OR                            
                ( CONSULTINGDR1ID = P_PHYSICIANNUMBER OR                        
                CONSULTINGDR2ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR3ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR4ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR5ID = P_PHYSICIANNUMBER ) ) AND                     
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_ATTENDING                                          
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND ATTENDINGDRID = P_PHYSICIANNUMBER AND                       
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_ADMITTING                                          
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND ADMITTINGDRID = P_PHYSICIANNUMBER AND                       
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_REFERING                                           
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND TRIM ( REFERINGDRID ) = REFDRID AND                         
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_OPERATING                                          
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND OPERATINGDRID = P_PHYSICIANNUMBER AND                       
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                SELECT COUNT ( ACCOUNTNUMBER )                                  
                INTO O_TOTAL_CONSULTING                                         
                FROM ACCOUNTPROXIES                                             
                WHERE FACILITYID = P_FACILITYID                                 
                AND ( CONSULTINGDR1ID = P_PHYSICIANNUMBER OR                    
                CONSULTINGDR2ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR3ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR4ID = P_PHYSICIANNUMBER OR                          
                CONSULTINGDR5ID = P_PHYSICIANNUMBER ) AND                       
                ( PATIENTTYPE = '1' ) AND                                       
                                ( ( DISCHARGEDATE IS NULL )                     
                OR ( DATE ( DISCHARGEDATE ) = CURRENT_DATE )                    
                OR ( PENDINGDISCHARGE = 'Y' ) ) ;                               
                                                                                
                END P1  ;                                                       
