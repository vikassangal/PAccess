
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: jimmy zhang                                                    */
-- * Started:  03/30/2005                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','GETACCTSBALANCESWPLAN');
--go


-- *****************************************************************************/
-- * SP Definition - PACCESS.GETACCTSBALANCESWPLAN                             */
-- *    Params     - P_HSP	        - an HSP number                            */
-- *    Params     - P_HSP	        - an HSP code                              */
-- *    Params     - P_MRC	        - a medical record number                  */
-- *****************************************************************************/

CREATE PROCEDURE PACCESS.GETACCTSBALANCESWPLAN ( 
	IN P_HSP     INTEGER,
	IN P_HSPCODE CHAR(3),
	IN P_MRC     INTEGER
	 ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.GETACCTSBALANCESWPLAN
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
	
	    DECLARE CURSOR1 CURSOR FOR 	        
		
		SELECT  a.LPLDD  as DISCHARGEDATE,
		        a.LPACCT as ACCOUNTNUMBER,			     
			    a.LPFC   as FINANCIALCLASSCODE, 			        
			    --t.QTKEY  as PATIENTTYPECODE, 
			    --t.QTTAD  as PATIENTTYPEDESC,
			    a.LPPTYP as PATIENTTYPECODE,
		        b.PMTDU9 as TOTALDUE
		FROM NMWHPLIBR.HPADLPUC a        
		--FROM HPDATA1 . HPADLPP a			
			left join PADATA.PM0001P b on ( b.PMHSPC = P_HSPCODE AND b.PMPT#9 = a.LPACCT )   
			--left join HPDATA1.HPADQTTA t on ( t.QTHSP# = a.LPHSP# AND t.QTKEY = a.LPPTYP )		
		WHERE a.LPHSP# = P_HSP AND --test use 900
		      a.LPMRC# = P_MRC AND --test use  785147
		      a.LPFC = '73'    AND --test use '80'
	          b.PMTDU9 >0; 	 		          					
		
		OPEN CURSOR1 ; 
		
		
		END P1  ;  