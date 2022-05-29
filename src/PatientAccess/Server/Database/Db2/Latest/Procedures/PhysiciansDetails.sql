                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PHYSDETAIL  - STORED PROCEDURE FOR PX                */        
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
/* 04/26/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
/* 07/06/2006 I                    I Modified STORED PROCEDURE        */        
/* 07/18/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
                                                                                
SET PATH *LIBL;                                                                 
                                                                                
CREATE PROCEDURE PHYSICIANSDETAILS (                                            
        IN P_FACILITYID INTEGER ,                                               
        IN P_PHYSICIANNUMBER INTEGER )                                          
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                             
        SPECIFIC PHYSDETAIL                                                      
        NOT DETERMINISTIC                                                        
        READS SQL DATA                                                           
        CALLED ON NULL INPUT                                                     
        SET OPTION DBGVIEW =*SOURCE                                             
        P1 : BEGIN                                                              
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT A . MRDR# AS PHYSICIANNUMBER ,                                   
        A . MRDNM AS PHYSICIANNAME ,                                            
        A . MRFNM AS PHYSICIANFIRSTNAME ,                                       
        A . MRLNM AS PHYSICIANLASTNAME ,                                        
        A . MRMI AS PHYSICIANMIDDLEINITIAL ,                                    
        A . MRDADR AS PHYSICIANADDRESS ,                                        
        A . MRDCIT AS CITY ,                                                    
        A . MRDSTE AS STATE ,                                                   
        A . MRDZIP AS ZIP ,                                                     
        A . MRSPCD AS PHYSICIANSPECIALITYCODE ,                                 
        A . MRDPH# AS PHYSICIANPHONENUMBER ,                                    
        A . MRINA AS PHYSICIANINACTIVAACTIVE ,                                  
        A . MRHSP# AS HOSPITALNUMBER ,                                          
        A . MRFLIC AS FEDERALLICENSENUMBER ,                                    
        A . MRSLIC AS STATELICENSENUMBER ,                                      
        A . MRMDAP AS ADMPRIVELEGES ,                                           
        A . MRMDG# AS MDGRPNO ,                                                 
        A . MRTITL AS TITLE ,                                                   
        A . MRDACD AS AREACODE ,                                                
        A . MRCPH# AS CELLPHONENO ,                                             
        A . MRCACD AS CELLPHONEAREACODE ,                                       
        A . MRBACD AS BEEPERAREACODE ,                                          
        A . MRBPR# AS BEEPERNUMBER ,                                            
        A . MRBPIN AS BEEPERPIN ,                                               
        A . MRSPCI AS SPECIALITYINDICATOR ,                                     
        A . MRUPI# AS UPIN ,                                                    
        A . MRSTAT AS ACTIVEORINACTIVE ,                                        
        A . MRDOS8 AS DATEACTIVATED ,                                           
        A . MRDLT8 AS DATEINACTIVATED , 
        A . MRNPRO AS NATIONALPROVIDERID,
        S . QTDSTD AS STATUSDESC ,                                              
        NDEX . EXDATE AS DATEEXCLUDED                                           
        FROM HPADMRP A                                                          
        LEFT OUTER JOIN NDEXMDP NDEX ON                                         
        ( NDEX . EXHSP# = A . MRHSP#                                            
        AND NDEX . EXDR# = A . MRDR# )                                          
                                                                                
        LEFT OUTER JOIN HPADQTDC S                                              
        ON A . MRHSP# = S . QTHSP#                                              
        AND A . MRSTAT = S . QTKEY                                              
        WHERE MRDR# = P_PHYSICIANNUMBER                                         
        AND MRHSP# = P_FACILITYID ;                                             
                                                                                
        OPEN CURSOR1 ;                                                          
                                                                                
        END P1  ;                                                               
