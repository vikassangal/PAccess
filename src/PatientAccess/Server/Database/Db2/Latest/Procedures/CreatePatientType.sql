                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  CREATEPATIENTTYPE - SQL FUNCTION FOR PX              */        
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
                                                                                
CREATE FUNCTION CREATEPATIENTTYPE (                                             
 LPPSTO VARCHAR(1) ,                                                            
 LPPSTI VARCHAR(1) ,                                                            
 LPFBIL VARCHAR(1) ,                                                            
 LPVIS# DECIMAL(5, 0) ,                                                         
 LPPOLN VARCHAR(1) ,                                                            
 LPUBAL DECIMAL(5, 0) ,                                                         
 LPMSV VARCHAR(3) )                                                             
 RETURNS CHAR(8)                                                                
 LANGUAGE SQL                                                                   
 SPECIFIC CRTPATTYP                                                             
 DETERMINISTIC                                                                  
 CONTAINS SQL                                                                   
 CALLED ON NULL INPUT                                                           
 NO EXTERNAL ACTION                                                             
 SET OPTION DBGVIEW = *SOURCE                                                   
 BEGIN ATOMIC                                                                   
                                                                                
  DECLARE INTERNALSTATUSCODE CHAR ( 1 ) ;                                       
  DECLARE RETURNVAL CHAR ( 8 ) DEFAULT '' ;                                     
                                                                                
  IF ( LPPSTI = 'A' AND LPPOLN = 'P' ) THEN                                     
  SET INTERNALSTATUSCODE = 'M' ;                                                
  ELSEIF ( LPPSTI = 'A' AND LPPOLN <> 'P' AND LPUBAL = 0 ) THEN                 
  SET INTERNALSTATUSCODE = 'A' ;                                                
  ELSEIF ( LPPSTI = 'A' AND LPPOLN <> 'P' AND LPUBAL <> 0 ) THEN                
  SET INTERNALSTATUSCODE = 'I' ;                                                
  ELSEIF ( LPPSTI = 'B' ) THEN                                                  
  SET INTERNALSTATUSCODE = 'B' ;                                                
  ELSEIF ( LPPSTI = 'C' OR LPPSTI = 'J' ) THEN                                  
  SET INTERNALSTATUSCODE = 'C' ;                                                
  ELSEIF ( LPPSTI = 'B' AND LPPSTO = ' ' ) THEN                                 
  SET INTERNALSTATUSCODE = 'G' ;                                                
  ELSEIF ( LPPSTI = 'H' ) THEN                                                  
  SET INTERNALSTATUSCODE = 'H' ;                                                
  ELSEIF ( LPPSTO = 'K' ) THEN                                                  
  SET INTERNALSTATUSCODE = 'K' ;                                                
  ELSEIF ( LPPSTO = 'E' AND LPFBIL = 'Y' ) THEN                                 
  SET INTERNALSTATUSCODE = 'L' ;                                                
  ELSEIF ( LPPSTO = 'E' AND LPFBIL <> 'Y' ) THEN                                
  SET INTERNALSTATUSCODE = 'E' ;                                                
  ELSEIF ( LPPSTO = 'F' AND LPFBIL = 'Y' ) THEN                                 
  SET INTERNALSTATUSCODE = 'L' ;                                                
  ELSEIF ( LPPSTO = 'F' AND LPFBIL <> 'Y' ) THEN                                
  SET INTERNALSTATUSCODE = 'D' ;                                                
  ELSEIF ( LPPSTO = 'D' AND LPVIS# = 0 ) THEN                                   
  SET INTERNALSTATUSCODE = 'D' ;                                                
  ELSEIF ( LPPSTO = 'D' AND LPVIS# <> 0 ) THEN                                  
  SET INTERNALSTATUSCODE = 'E' ;                                                
  END IF ;                                                                      
                                                                                
  IF ( INTERNALSTATUSCODE = 'M' ) THEN                                          
  SET RETURNVAL = 'PRE-PUR' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'A' ) THEN                                      
  SET RETURNVAL = 'PRE-N/C' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'I' ) THEN                                      
  SET RETURNVAL = 'PRE-CHG' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'B' ) THEN                                      
  SET RETURNVAL = 'INP-INH' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'C' ) THEN                                      
  SET RETURNVAL = 'INP-DIS' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'E' ) THEN                                      
  SET RETURNVAL = 'OUT-REC' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'G' ) THEN                                      
  SET RETURNVAL = 'PRE-CAN' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'H' ) THEN                                      
  SET RETURNVAL = 'INP-CAN' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'K' ) THEN                                      
  SET RETURNVAL = 'OUT-PRE' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'L' ) THEN                                      
  SET RETURNVAL = 'OUT-FIN' ;                                                   
  ELSEIF ( INTERNALSTATUSCODE = 'D' ) THEN                                      
  SET RETURNVAL = 'O HSV' || LPMSV ;                                            
  END IF ;                                                                      
                                                                                
  RETURN RETURNVAL ;                                                            
                                                                                
  END  ;                                                                        
                                                                                
                                                                                
