                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTDETAILACCOUNTINFO - SQL PROC FOR PX            */        
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
                                                                                
CREATE PROCEDURE SELECTDETAILACCOUNTINFO (                                      
        IN P_HSP INTEGER ,                                                      
        IN P_ACCOUNTNUMBER INTEGER )                                            
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELDTLACC                                                      
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                        DECLARE CURSOR1 CURSOR FOR                              
                        SELECT                                                  
                        A.LPHSP# AS HSPNUMBER ,                                 
                        A.LPACCT AS ACCOUNTNUMBER ,                             
                        A.LPALOC AS ACCIDENTLOCATION ,                          
                        A.LPATIM AS ACCIDENTTIME ,                              
                        A.LPACTP AS ACCIDENTTYPE ,                              
                        A.LPDIAG AS DIAGNOSIS ,                                 
                        A.LPACST AS AUTOACCIDENTSTATE ,                         
                        A.LPACCN AS ACCIDENTCOUNTRYCODE ,                       
                        A.LPLAT AS TIMEOFADMISSION ,                            
                        A.LPOC01 AS OCCURANCECODE1 ,                            
                        A.LPOC02 AS OCCURANCECODE2 ,                            
                        A.LPOC03 AS OCCURANCECODE3 ,                            
                        A.LPOC04 AS OCCURANCECODE4 ,                            
                        A.LPOC05 AS OCCURANCECODE5 ,                            
                        A.LPOC06 AS OCCURANCECODE6 ,                            
                        A.LPOC07 AS OCCURANCECODE7 ,                            
                        A.LPOC08 AS OCCURANCECODE8 ,                            
                        A.LPOA01 AS OCCURANCEDATE1 ,                            
                        A.LPOA02 AS OCCURANCEDATE2 ,                            
                        A.LPOA03 AS OCCURANCEDATE3 ,                            
                        A.LPOA04 AS OCCURANCEDATE4 ,                            
                        A.LPOA05 AS OCCURANCEDATE5 ,                            
                        A.LPOA06 AS OCCURANCEDATE6 ,                            
                        A.LPOA07 AS OCCURANCEDATE7 ,                            
                        A.LPOA08 AS OCCURANCEDATE8 ,                            
                        A.LPCLVS AS CLERGYVISIT ,                               
                        A.LPLMEN AS LASTMENSTRATION ,                           
                        A.LPWRNF AS FACILITYFLAG ,                              
                        A.LPPSTO AS OPSTATUSCODE ,                              
                        A.LPPSTI AS IPSTATUSCODE ,                              
                        A.LPFBIL AS FINALBILLINGFLAG ,                          
                        A.LPVIS# AS OPVISITNUMBER ,                             
                        A.LPPOLN AS PENDINGPURGE ,                              
                        A.LPUBAL AS UNBILLEDBALANCE ,                           
                        A.LPMSV AS MEDICALSERVICECODE ,                         
                        A.LPADT1 AS ADMITMONTH ,                                
                        A.LPADT2 AS ADMITDAY ,                                  
                        A.LPADT3 AS ADMITYEAR ,                                 
                        --a.LPLAT    AS TIMEOFADMISSION,                        
                        A.LPLDD AS DISCHARGEDATE ,                              
                        A.LPFC AS FINANCIALCLASSCODE ,                          
                        A.LPPTYP AS PATIENTTYPE ,                               
                        A.LPDCOD AS DISCHARGEDISPOSITIONCODE ,                  
                        A.LPSPNC AS FIRSTSPANCODE ,                             
                        A.LPSPFR AS FIRSTSPANFROMDATE ,                         
                        A.LPSPTO AS FIRSTSPANTODATE ,                           
                        A.LPINSN AS FIRSTSPANFACILITY ,                         
                        A.LPSPN2 AS SECONDSPANCODE ,                            
                        A.LPAPFR AS SECONDSPANFROMDATE ,                        
                        A.LPAPTO AS SECONDSPANTODATE ,                          
                        S.QTKEY AS MEDICALSERVICECODE ,                         
                        S.QTMSVD AS MEDICALSERVICEDESC ,                        
                        SUP.RVPRGI AS ISPREGNANT ,                              
                        FUS.PMTCH8 AS TOTALCHARGES ,                            
                        MPI.RWCNFG AS CONFIDENTIALFLAG ,                        
                        D.MDNPPV AS NPPVERSION ,                                
                        D.MDNPPF AS OPTOUTFLAGS ,                               
                        D.MDSMOK AS SMOKER ,                                    
                        D.MDBLDL AS BLOODLESS ,                                 
                        D.MDRORG AS RESISTANTORGANISM ,                         
                        C.LACMT1 AS COMMENTLINE1 ,                              
                        C.LACMT2 AS COMMENTLINE2                                
                        FROM HPADLPP A                                          
                        JOIN NOHLHSPP FAC ON                                    
                        ( FAC.HHHSPN = P_HSP )                                  
                        JOIN HPADMDP D ON                                       
                        ( D.MDHSP# = P_HSP                                      
                        AND D.MDMRC# = A.LPMRC# )                               
                        LEFT OUTER JOIN HPADLAP9 C ON                           
                        ( C.LAHSP# = P_HSP                                      
                        AND C.LAACCT = P_ACCOUNTNUMBER )                        
                        LEFT OUTER JOIN PM0001P FUS ON                          
                        ( FUS.PMPT#9 = P_ACCOUNTNUMBER                          
                        AND FUS.PMHSPC = FAC.HHHSPC )                           
                        LEFT OUTER JOIN HXMRRV2P SUP ON                         
                        ( SUP.RVHSP# = P_HSP                                    
                        AND SUP.RVPT#9 = A.LPACCT )                             
                        LEFT JOIN HPADQTMS S ON S.QTHSP# = P_HSP                
                        LEFT OUTER JOIN HXMRRWP MPI ON                          
                        ( MPI.RWHSP# = P_HSP                                    
                        AND MPI.RWACCT = P_ACCOUNTNUMBER )                      
                        WHERE A.LPHSP# = P_HSP                                  
                        AND A.LPACCT = P_ACCOUNTNUMBER                          
                        AND A.LPMSV = S.QTKEY ;                                 
                                                                                
                        OPEN CURSOR1 ;                                          
                                                                                
                        END P1  ;                                               
