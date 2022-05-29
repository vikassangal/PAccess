  /*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  PLANISVALID - SQL FUNCTION FOR PX				      */        
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
/* 02/08/2008 I	 Sophie Zhang	   I  new utility funciton            */       
/*************I********************I***********************************/     
 
 SET PATH *LIBL ; 

CREATE FUNCTION DATETOCHARYYYYMMDD ( 
	@P_DATE DATE ) 
	RETURNS DECIMAL(8,0)   
	LANGUAGE SQL 
	SPECIFIC DATETOCHAR 
	DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	NO EXTERNAL ACTION 

	BEGIN
		DECLARE DATECHAR VARCHAR(10) DEFAULT '';
		
		SET DATECHAR = CHAR(@P_DATE);
		 
		RETURN DECIMAL( SUBSTRING(DATECHAR, 1, 4) || 
						SUBSTRING(DATECHAR, 6, 2) ||
						SUBSTRING(DATECHAR, 9, 2) ); 
		END;
			
