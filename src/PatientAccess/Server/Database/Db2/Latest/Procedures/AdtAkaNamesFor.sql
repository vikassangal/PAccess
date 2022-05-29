                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  ADTAKANAMESFOR - SQL PROC FOR PATIENT ACCESS         */        
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
/* 04/27/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE ADTAKANAMESFOR (                                               
        IN P_HSP INTEGER ,                                                      
        IN P_MRC INTEGER )                                                      
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC ADTAKANME                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        AKA.AKHSP# AS HOSPITALNUMBER    ,                       
                        AKA.AKMRC# AS MEDICALRECORD     ,                       
                        AKA.AKPLNM AS PATIENTLASTNAME   ,                       
                        AKA.AKPFNM AS PATIENTFIRSTNAME  ,                       
                        AKA.AKTITL AS TITLE     ,                               
                        AKA.AKEDAT AS ENTRYDATE ,                               
                        AKA.AKSECF AS SECURITYFLAG                              
                                                                                
                        FROM NMNHAKAP AKA                                       
                        WHERE ( AKA.AKHSP# = P_HSP )                            
                        AND ( AKA.AKMRC# = P_MRC ) ;                            
                        OPEN CURSOR1 ;                                          
                        END P1  ;                                               
