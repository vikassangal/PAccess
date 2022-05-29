                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELNSTAFIN - SQL PROC FOR PX                         */        
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
/* 08/02/2006 I                    I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
CREATE PROCEDURE SELECTNONSTAFFPHYSICIANINFO (                                  
                                                                                
        IN P_HSP INTEGER ,                                                      
                                                                                
        IN P_ACCOUNTNUMBER INTEGER )                                            
                                                                                
        DYNAMIC RESULT SETS 1                                                   
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC SELNSTAFIN                                                     
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                                                                                
                SELECT                                                          
                                                                                
                        LAACCT AS ACCOUNTNUMBER ,                               
                        LAHSP# AS HOSPITALNUMBER ,                               
                        LADEL AS DELETECODE ,                                    
                        LAMNU# AS ADMITTINGNSPUPIN ,                             
                        LAMNDN AS ADMITTINGNSPNAME ,                             
                        LAMNFR AS ADMITTINGNSPNUM ,                              
                        LAMNCD AS ADMITTINGNSPAREACODE ,                          
                        LAMNP# AS ADMITTINGNSPPHONENUM ,                         
                        LAMNLN AS ADMITTINGNSPLASTNAME ,                            
                        LAMNFN AS ADMITTINGNSPFIRSTNAME ,                        
                        LAMNMN AS ADMITTINGNSPMIDINITIAL ,                       
                        LAMNPR AS ADMITTINGNSPNATLPROVID ,                       
                        LAMTXC AS ADMITTINGNSPTAXONOMYCD ,                        
                        LAMSL# AS ADMITTINGNSPSTATELICNO ,                      
                        LARNU# AS REFERRINGNSPUPIN ,                             
                        LARNDN AS REFERRINGNSPNAME ,                             
                        LARNFR AS REFERRINGNSPFINR ,                             
                        LARNLN AS REFERRINGNSPLASTNAME ,                          
                        LARNFN AS REFERRINGNSPFIRSTNAME ,                        
                        LARNMN AS REFERRINGNSPMIDINITIAL ,                       
                        LARNPR AS REFERRINGNSPNATLPROVID ,                         
                        LARTXC AS REFERRINGNSPTAXONOMYCD ,                       
                        LARNP# AS REFERRINGNSPPHONENUM ,                         
                        LARSL# AS REFFERRINGNSPSTATELICNO ,                      
                        LAANLN AS ATTENDINGNSPLASTNAME ,                        
                        LAANFN AS ATTENDINGNSPFIRSTNAME ,                        
                        LAANMN AS ATTENDINGNSPMIDINITIAL ,                       
                        LAANU# AS ATTENDINGNSPUPIN ,                            
                        LAANPR AS ATTENDINGNSPNATLPROVID ,                      
                        LAATXC AS ATTENDINGNSPTAXONOMYCD ,                        
                        LAANP# AS ATTENDINGNSPPHONENUM ,                         
                        LAASL# AS ATTENDINGSTATELICNO ,                          
                        LAONLN AS OPRERATINGNSPLASTNAME ,                        
                        LAONFN AS OPRERATINGNSPFIRSTNAME ,                       
                        LAONMN AS OPRERATINGNSPMIDINITIAL ,                      
                        LAONU# AS OPRERATINGNSPUPIN ,                            
                        LAONPR AS OPRERATINGNSPNATLPROVID ,                        
                        LAOTXC AS OPRERATINGNSPTAXONOMYCD ,                       
                        LAONP# AS OPRERATINGNSPPHONENUM ,                        
                        LAOSL# AS OPRERATINGNSPSTATELICNO ,                      
                        LATNLN AS OTHERNSPLASTNAME ,                            
                        LATNFN AS OTHERNSPFIRSTNAME ,                           
                        LATNMN AS OTHERNSPMIDINITIAL ,                          
                        LATNU# AS OTHERNSPUPIN ,                                
                        LATNPR AS OTHERNSPNATLPROVID ,                          
                        LATTXC AS OTHERNSPTAXONOMYCD ,                          
                        LATNP# AS OTHERNSPPHONENUM ,                             
                        LATSL# AS OTHERNSPSTATELICNO ,                          
                        LATECR AS TENETCARE ,                                   
                        LAWSID AS WORKSTATIONID ,                                
                        LAEMID AS EMPLOYEEID                                       
                FROM HPADLAP9 H                                                 
                WHERE H . LAHSP# = P_HSP                                        
                AND H . LAACCT = P_ACCOUNTNUMBER ;                              
                                                                                
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
