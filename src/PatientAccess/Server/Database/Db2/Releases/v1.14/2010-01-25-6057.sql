                                                                                
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
/* 01/25/2010 I Deepa Raju		   I SR446 - Drop stored procedures   */
/*            I                    I for Research Sponsors. They      */
/*            I                    I have been renamed as             */
/*            I                    I SELECTALLRESEARCHSTUDIES and     */
/*            I                    I SELECTRESEARCHSTUDYWITH          */
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                          
DROP PROCEDURE SELECTALLRESEARCHSPONSORS ( INT );
DROP PROCEDURE SELECTRESEARCHSPONSORMWITH ( INT, VARCHAR(10) ) ;