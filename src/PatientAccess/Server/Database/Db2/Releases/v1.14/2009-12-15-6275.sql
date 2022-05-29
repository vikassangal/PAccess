                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  Drop SELECTALLRESISTANTORGANISMS - SQL PROC FOR PX   */        
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
/* 11/10/2009 I Smitha			   Drop stored procedures related to  */
/*								   Resistant Organism table(HPADQTOR).*/
/*                         These are not be used any more after SR 348*/        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                          
DROP PROCEDURE SELECTALLRESISTANTORGANISMS ();
DROP PROCEDURE SELECTRESISTANTORGANISMWITH ( VARCHAR(10) ) ;