                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  DELETEEMPLOYER - SQL PROC FOR PX                     */        
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
/* 11/19/2008 I  Sanjeev Kumar     I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE DELETEEMPLOYERFORAPPROVAL (                                               
        IN P_EMCODE INTEGER )                                                   
        LANGUAGE SQL                                                            
        SPECIFIC DLEMPFRAPP                                                         
        NOT DETERMINISTIC                                                       
        MODIFIES SQL DATA                                                       
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
                        DELETE FROM EMPLOYERSFORAPPROVAL                                     
                        WHERE EMCODE = P_EMCODE ;        
                        
						DELETE FROM EMPLOYERADDRESSFORAPPROVAL EADFA
							WHERE	EADFA.EMCODE = P_EMCODE ;
                                               
                        END P1  ;                                               
 